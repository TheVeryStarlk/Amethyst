using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Eventing;
using Amethyst.Networking;
using Amethyst.Networking.Serializers;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, EventDispatcher eventDispatcher, Socket socket) : IClient, IDisposable
{
    private readonly NetworkStream stream = new(socket);
    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets)
    {
        if (source.IsCancellationRequested)
        {
            return;
        }

        foreach (var packet in packets)
        {
            if (!outgoing.Writer.TryWrite(packet))
            {
                break;
            }
        }
    }

    public void Stop()
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
        socket.Dispose();
    }

    private async Task WritingAsync()
    {
        var output = PipeWriter.Create(stream);

        // Serializers return the packet's length without accounting for the identifier and the total packet length,
        // which are both encoded as variable integers that take at most five bytes.
        const int extra = sizeof(int) + sizeof(int) + sizeof(short);

        await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                var serializer = packet.Create();
                var span = output.GetSpan(serializer.Length + extra);

                output.Advance(Protocol.Write(span, packet, serializer));
                await output.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An exception occurred while writing");
                break;
            }
        }
    }
}