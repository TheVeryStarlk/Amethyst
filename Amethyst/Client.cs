using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Events.Minecraft.Player;
using Amethyst.Entities;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Handshaking;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Networking.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<Client> logger,
    Server server,
    ConnectionContext connection,
    int identifier) : IAsyncDisposable
{
    public int Identifier => identifier;

    public Server Server => server;

    public Transport Transport { get; } = new Transport(connection.Transport);

    public ClientState State { get; private set; }

    public Player? Player { get; private set; }

    public int MissedKeepAliveCount { get; set; }

    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public async Task StartAsync()
    {
        while (true)
        {
            try
            {
                var message = await Transport.ReadAsync(source.Token);

                if (message is null)
                {
                    State = ClientState.Disconnected;
                    break;
                }

                var task = State switch
                {
                    ClientState.Handshaking => HandleHandshakingAsync(message),
                    ClientState.Status => HandleStatusAsync(message),
                    ClientState.Login => HandleLoginAsync(message),
                    ClientState.Playing => HandlePlayingAsync(message),
                    ClientState.Disconnected => Task.CompletedTask,
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
                if (Player is not null)
                {
                    await Player.DisconnectAsync(ChatMessage.Create("Internal server error.", Color.Red));
                }

                logger.LogError(
                    "Unexpected exception while handling packets: \"{Message}\"",
                    exception);

                break;
            }
        }

        await HandleDisconnectedAsync();

        connection.Abort();
        logger.LogDebug("Client stopped {Identifier}", identifier);
    }

    public async Task StopAsync()
    {
        if (State is ClientState.Disconnected)
        {
            return;
        }

        State = ClientState.Disconnected;
        await source.CancelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();
        await connection.DisposeAsync();
    }

    private async Task HandleHandshakingAsync(Message message)
    {
        var handshake = message.As<HandshakePacket>();
        await handshake.HandleAsync(this);

        State = handshake.NextState;
        logger.LogDebug("Client switched state to {State}", State);
    }

    private async Task HandleStatusAsync(Message message)
    {
        var task = message.Identifier switch
        {
            0x00 => message.As<StatusRequestPacket>().HandleAsync(this),
            0x01 => message.As<PingRequestPacket>().HandleAsync(this),
            _ => throw new InvalidOperationException("Unknown packet.")
        };

        await task;

        if (message.Identifier == 0x01)
        {
            await StopAsync();
        }
    }

    private async Task HandleLoginAsync(Message message)
    {
        var loginStart = message.As<LoginStartPacket>();

        Player = new Player(this, loginStart.Username)
        {
            Position = new VectorF(0, 8, 0),
            GameMode = GameMode.Creative
        };

        await loginStart.HandleAsync(this);

        State = ClientState.Playing;
        logger.LogDebug("Login success with username: \"{Username}\"", Player.Username);
    }

    private async Task HandlePlayingAsync(Message message)
    {
        var task = message.Identifier switch
        {
            0x00 => message.As<KeepAlivePacket>().HandleAsync(this),
            0x01 => message.As<ChatMessagePacket>().HandleAsync(this),
            0x03 => message.As<OnGroundPacket>().HandleAsync(this),
            0x04 => message.As<PlayerPositionPacket>().HandleAsync(this),
            0x05 => message.As<PlayerLookPacket>().HandleAsync(this),
            0x06 => message.As<PlayerPositionAndLookPacket>().HandleAsync(this),
            0x07 => message.As<PlayerDiggingPacket>().HandleAsync(this),
            0x08 => message.As<PlayerBlockPlacementPacket>().HandleAsync(this),
            0x14 => message.As<TabCompletePacket>().HandleAsync(this),
            0x15 => message.As<ClientSettingsPacket>().HandleAsync(this),
            _ => Task.CompletedTask
        };

        await task;
    }

    private async Task HandleDisconnectedAsync()
    {
        if (Player is null)
        {
            return;
        }

        await Player.World!.RemovePlayerAsync(Player);

        var eventArgs = await Server.EventService.ExecuteAsync(
            new PlayerLeftEventArgs
            {
                Server = Server,
                Player = Player,
                Message = ChatMessage.Create($"{Player.Username} has left the server.", Color.Yellow)
            });

        await Server.BroadcastChatMessageAsync(eventArgs.Message);
    }
}

internal enum ClientState
{
    Handshaking,
    Status,
    Login,
    Playing,
    Disconnected
}