using Amethyst.Components;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, ConnectionContext connection, EventDispatcher eventDispatcher) : IClient, IDisposable
{
    public int Identifier { get; } = Random.Shared.Next();

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly ProtocolWriter writer = new(connection.Transport.Output);
    private readonly SemaphoreSlim semaphore = new(1);

    public async ValueTask WriteAsync(params IOutgoingPacket[] packets)
    {
        await semaphore.WaitAsync(source.Token).ConfigureAwait(false);

        try
        {
            foreach (var packet in packets)
            {
                await writer.WriteAsync(packet, source.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Nothing.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected exception while writing to client");
        }
        finally
        {
            semaphore.Release();
        }
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