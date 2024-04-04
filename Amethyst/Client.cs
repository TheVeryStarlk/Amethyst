using Amethyst.Api.Entities;
using Amethyst.Entities;
using Amethyst.Extensions;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshaking;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<IClient> logger,
    int identifier,
    Server server,
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

    public IPlayer? Player { get; private set; }

    public int Identifier { get; } = identifier;

    private CancellationTokenSource? source;
    private DateTimeOffset idle;
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
            catch (OperationCanceledException)
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
    }

    public async Task TickAsync()
    {
        if (Player is null)
        {
            return;
        }

        if (DateTimeOffset.Now.Subtract(idle) > server.Options.IdleTimeOut)
        {
            await Player.KickAsync();
        }

        // Hmm, I wonder if order matters.
        await queue
            .Select(packet => transport.WriteAsync(packet))
            .WhenEach();
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

    private Task HandleStatusAsync(Message message)
    {
        return Task.CompletedTask;
    }

    private Task HandleLoginAsync(Message message)
    {
        Player = new Player(server, this);
        return Task.CompletedTask;
    }

    private Task HandlePlayingAsync(Message message)
    {
        return Task.CompletedTask;
    }
}

internal interface IClient
{
    public IPlayer? Player { get; }

    public void Queue(IOutgoingPacket packet);

    public void Stop();
}