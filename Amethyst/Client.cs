using System.Threading.Channels;
using Amethyst.Components;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, ConnectionContext connection, EventDispatcher eventDispatcher, int identifier) : IClient, IAsyncDisposable
{
    public int Identifier => identifier;

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly (ProtocolReader Input, ProtocolWriter Output) protocol = connection.CreateProtocol();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private string reason = "No reason provided.";

    public Task StartAsync()
    {
        return Task.WhenAll(ReadingAsync(), WritingAsync());
    }

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets)
    {
        foreach (var packet in packets)
        {
            if (!outgoing.Writer.TryWrite(packet))
            {
                break;
            }
        }
    }

    public void Stop(string message)
    {
        reason = message;
        source.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        source.Dispose();
        return connection.DisposeAsync();
    }

    private async Task ReadingAsync()
    {
        while (true)
        {
            try
            {
                var message = await protocol.Input.ReadAsync(source.Token);
                await eventDispatcher.DispatchAsync(this, new Received(message), source.Token);
                protocol.Input.Advance();
            }
            catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected exception while reading from client");
                break;
            }
        }
    }

    private async Task WritingAsync()
    {
        try
        {
            await foreach (var packet in outgoing.Reader.ReadAllAsync(source.Token))
            {
                await protocol.Output.WriteAsync(packet, source.Token);
            }
        }
        catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
        {
            // Nothing.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected exception while writing to client");
        }
    }
}