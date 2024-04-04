using Amethyst.Api.Entities;
using Amethyst.Protocol;
using Microsoft.AspNetCore.Connections;

namespace Amethyst;

internal sealed class Client(Server server, ConnectionContext connection) : IClient, IAsyncDisposable
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

    private CancellationTokenSource? source;
    private State state;
    private DateTimeOffset idle;

    private readonly Queue<IOutgoingPacket> queue = [];

    public void Start()
    {
        source = new CancellationTokenSource();

        while (!source.IsCancellationRequested)
        {
            // Read...
        }
    }

    public void Tick()
    {
        if (idle.Subtract(DateTimeOffset.Now) > server.Options.IdleTimeOut)
        {
            Player?.Kick();
        }

        foreach (var item in queue)
        {
            // Do something.
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

    private void HandeDisconnected()
    {
        state = State.Disconnected;

        Player?.Kick();
        connection.Abort();
    }
}

internal interface IClient
{
    public IPlayer? Player { get; }

    public void Queue(IOutgoingPacket packet);

    public void Stop();
}