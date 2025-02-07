using System.IO.Pipelines;

namespace Amethyst.Protocol;

public sealed class ProtocolWriter(PipeWriter output)
{
    public async ValueTask WriteAsync(IOutgoingPacket packet, CancellationToken cancellationToken)
    {
        // Length is used to denote the packet's length including the identifier,
        // total includes the byte count of length and the value of length itself.
        var length = Variable.GetByteCount(packet.Identifier) + packet.Length;
        var total = Variable.GetByteCount(length) + length;

        var span = output.GetSpan(total);
        var writer = new SpanWriter(span);

        writer.WriteVariableInteger(length);
        writer.WriteVariableInteger(packet.Identifier);

        packet.Write(span[length..]);

        output.Advance(total);

        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}