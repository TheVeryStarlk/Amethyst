using Amethyst.Api.Entities;
using Amethyst.Extensions;
using Amethyst.Protocol;
using Amethyst.Protocol.Transport;
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

    public IPlayer? Player { get; }

    public int Identifier { get; } = identifier;

    private CancellationTokenSource? source;
    private DateTimeOffset idle;
    private State state;

    private readonly Queue<IOutgoingPacket> queue = [];
    private readonly Transport transport = new Transport(connection.Transport);

    public async Task StartAsync()
    {
        source = new CancellationTokenSource();

        logger.LogDebug("Started connection");

        while (!source.IsCancellationRequested)
        {
            try
            {
                var message = await transport.ReadAsync(source.Token);

                if (message is null)
                {
                    break;
                }
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

        await HandleDisconnectedAsync();
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

    private async Task HandleDisconnectedAsync()
    {
        state = State.Disconnected;

        if (Player is not null)
        {
            await Player.KickAsync();
        }

        connection.Abort();
    }
}

internal interface IClient
{
    public IPlayer? Player { get; }

    public void Queue(IOutgoingPacket packet);

    public void Stop();
}