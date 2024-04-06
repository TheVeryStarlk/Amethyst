using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins.Events.Server;
using Amethyst.Components;
using Amethyst.Entities;
using Amethyst.Extensions;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshaking;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Playing;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<IClient> logger,
    int identifier,
    IServer server,
    ConnectionContext connection) : IClient, IAsyncDisposable
{
    private enum State
    {
        Handshaking,
        Status,
        Login,
        Playing,
        Disconnected
    }

    public int Identifier => identifier;

    public IPlayer? Player { get; private set; }

    public DateTimeOffset Idle { get; set; }

    private CancellationTokenSource? source;
    private State state;

    private readonly Queue<IOutgoingPacket> queue = [];
    private readonly Transport transport = new Transport(connection.Transport);

    public async Task StartAsync()
    {
        source = new CancellationTokenSource();
        state = State.Handshaking;

        logger.LogDebug("Started connection");

        while (true)
        {
            try
            {
                var message = await transport.ReadAsync(source.Token);

                if (message is null)
                {
                    break;
                }

                var task = state switch
                {
                    State.Handshaking => HandleHandshakingAsync(message),
                    State.Status => HandleStatusAsync(message),
                    State.Login => HandleLoginAsync(message),
                    State.Playing => HandlePlayingAsync(message),
                    State.Disconnected => Task.CompletedTask,
                    _ => throw new ArgumentOutOfRangeException()
                };

                await task;
            }
            catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception while handling packets: \"{Message}\"",
                    exception);

                break;
            }
        }

        state = State.Disconnected;

        if (Player is not null)
        {
            await Player.KickAsync();
        }

        connection.Abort();

        logger.LogDebug("Client stopped");
    }

    public async Task TickAsync()
    {
        if (Player is null)
        {
            return;
        }

        if (DateTimeOffset.Now.Subtract(Idle) > server.Options.IdleTimeOut)
        {
            await Player.KickAsync();
        }

        queue.Enqueue(
            new KeepAlivePacket
            {
                Payload = queue.Count
            });

        try
        {
            // Hmm, I wonder if order matters.
            await queue
                .Select(packet => transport.WriteAsync(packet))
                .WhenEach();
        }
        catch
        {
            // Connection has been aborted, ignore.
        }
        finally
        {
            queue.Clear();
        }
    }

    public void Queue(IOutgoingPacket packet)
    {
        queue.Enqueue(packet);
    }

    public void Stop()
    {
        source?.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        source?.Dispose();
        await connection.DisposeAsync();
    }

    private Task HandleHandshakingAsync(Message message)
    {
        var handshake = message.As<HandshakePacket>();

        state = (State) handshake.NextState;

        if (handshake.ProtocolVersion != server.ProtocolVersion)
        {
            Stop();
        }

        return Task.CompletedTask;
    }

    private async Task HandleStatusAsync(Message message)
    {
        switch (message.Identifier)
        {
            case 0x00:
                _ = message.As<StatusRequestPacket>();

                var @event = await server.EventService.ExecuteAsync(
                    new ServerDescriptionRequestEvent
                    {
                        Server = server,
                        Description = server.Options.Description
                    });

                await transport.WriteAsync(
                    new StatusResponsePacket
                    {
                        Status = ServerStatus.Create(
                            nameof(Amethyst),
                            server.ProtocolVersion,
                            server.Options.MaximumPlayers,
                            server.Players.Count(),
                            @event.Description)
                    });

                return;

            case 0x01:
            {
                var ping = message.As<PingRequestPacket>();

                await transport.WriteAsync(
                    new PongResponsePacket
                    {
                        Payload = ping.Payload
                    });

                break;
            }
        }

        Stop();
    }

    private async Task HandleLoginAsync(Message message)
    {
        var start = message.As<LoginStartPacket>();

        var player = new Player(this)
        {
            Server = server,
            World = server.Worlds.Values.First(),
            Guid = Guid.NewGuid(),
            Username = start.Username,
            GameMode = GameMode.Creative
        };

        await transport.WriteAsync(
            new LoginSuccessPacket
            {
                Guid = player.Guid,
                Username = player.Username
            });

        IOutgoingPacket[] packets =
        [
            new JoinGamePacket
            {
                Player = player
            },
            new PlayerPositionAndLookPacket
            {
                Position = player.Position,
                Yaw = player.Yaw,
                Pitch = player.Pitch,
                OnGround = player.OnGround
            }
        ];

        foreach (var packet in packets)
        {
            Queue(packet);
        }

        state = State.Playing;

        Player = player;
        Idle = DateTimeOffset.Now;
    }

    private async Task HandlePlayingAsync(Message message)
    {
        var task = message.Identifier switch
        {
            0x00 => message.As<KeepAlivePacket>().HandleAsync(server, Player!, this),
            0x01 => message.As<ChatPacket>().HandleAsync(server, Player!, this),
            _ => Task.CompletedTask
        };

        await task;
    }
}

internal interface IClient
{
    public IPlayer? Player { get; }

    public DateTimeOffset Idle { get; set; }

    public void Queue(IOutgoingPacket packet);

    public void Stop();
}