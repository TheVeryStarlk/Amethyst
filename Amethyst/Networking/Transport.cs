using System.IO.Pipelines;
using System.Threading.Channels;
using Amethyst.Extensions;

namespace Amethyst.Networking;

internal sealed class Transport(IDuplexPipe duplexPipe)
{
    private readonly Channel<IOutgoingPacket> packets = Channel.CreateUnbounded<IOutgoingPacket>();

    public async Task<Message?> ReadAsync(CancellationToken cancellationToken)
    {
        return await duplexPipe.Input.ReadMessageAsync(cancellationToken);
    }

    public async Task WriteAsync(IOutgoingPacket packet)
    {
        await duplexPipe.Output.WritePacketAsync(packet);
    }

    public void Queue(IOutgoingPacket packet)
    {
        _ = packets.Writer.TryWrite(packet);
    }

    public async Task DequeueAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (packets.Reader.TryRead(out var packet))
            {
                await WriteAsync(packet);
            }
            else
            {
                break;
            }
        }
    }
}