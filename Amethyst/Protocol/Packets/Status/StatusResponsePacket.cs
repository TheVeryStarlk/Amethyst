namespace Amethyst.Protocol.Packets.Status;

internal sealed record StatusResponsePacket(string Message) : StatusResponsePacketBase(Message), IWriteable
{
    public int Length => Variable.GetByteCount(Message);

    public void Write(Span<byte> span)
    {
        var writer = new SpanWriter(span);
        writer.WriteVariableString(Message);
    }
}