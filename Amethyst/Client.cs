using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Protocol;
using Microsoft.AspNetCore.Connections;

namespace Amethyst;

internal sealed class Client(ConnectionContext connection) : IClient, IDisposable
{
    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly SemaphoreSlim semaphore = new(1);

    public ValueTask WriteAsync(params IOutgoingPacket[] packets)
    {
        return ValueTask.CompletedTask;
    }

    public void Stop(Message message)
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
        semaphore.Dispose();
    }
}