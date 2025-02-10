using System.IO.Pipelines;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol;

internal sealed class ProtocolWriter(PipeWriter output)
{
    public async ValueTask WriteAsync(IOutgoingPacket packet)
    {
        var identifier = Variable.GetByteCount(packet.Identifier);

        var length = identifier + packet.Length;
        var body = Variable.GetByteCount(length);

        var total = body + length;
        var span = output.GetSpan(total);

        SpanWriter
            .Create(span)
            .WriteVariableInteger(length)
            .WriteVariableInteger(packet.Identifier);

        var index = body + identifier;
        packet.Write(span[index..]);

        output.Advance(total);

        await output.FlushAsync().ConfigureAwait(false);
    }
}