using System.IO.Pipelines;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol;

internal sealed class ProtocolWriter(PipeWriter output)
{
    public async ValueTask WriteAsync(IOutgoingPacket packet, CancellationToken cancellationToken)
    {
        var identifier = Variable.GetByteCount(packet.Identifier);

        var length = identifier + packet.Length;
        var body = Variable.GetByteCount(length);

        var total = body + length;

        var span = output.GetSpan(total);
        var writer = new SpanWriter(span);

        writer.WriteVariableInteger(length);
        writer.WriteVariableInteger(packet.Identifier);

        var index = body + identifier;
        packet.Write(span[index..]);

        output.Advance(total);

        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}