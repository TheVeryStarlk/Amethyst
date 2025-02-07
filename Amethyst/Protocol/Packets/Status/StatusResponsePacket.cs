using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Status;

public sealed record StatusResponsePacket(string Message) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Message);

    public void Write(Span<byte> span)
    {
        var writer = new SpanWriter(span);
        writer.WriteVariableString(Message);
    }
}