using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Status;

public sealed record StatusResponsePacket(string Message) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Message);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Message);
    }
}