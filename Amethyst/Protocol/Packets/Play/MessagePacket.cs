namespace Amethyst.Protocol.Packets.Play;

internal sealed record MessagePacket(string Message, byte Position)
    : MessagePacketBase(Message, Position), ICreatable<MessagePacketBase>, IWriteable
{
    public int Length => Variable.GetByteCount(Message) + sizeof(byte);

    public static MessagePacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new MessagePacket(reader.ReadVariableString(), 0);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Message).WriteByte(Position);
    }
}