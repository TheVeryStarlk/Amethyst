namespace Amethyst.Protocol.Packets.Play;

public sealed record MessagePacket(string Message, byte Position) : IIngoingPacket<MessagePacket>, IOutgoingPacket
{
    static int IIngoingPacket<MessagePacket>.Identifier => 1;

    public int Identifier => 2;

    int IOutgoingPacket.Length => Variable.GetByteCount(Message) + sizeof(byte);

    static MessagePacket IIngoingPacket<MessagePacket>.Create(SpanReader reader)
    {
        return new MessagePacket(reader.ReadVariableString(), 0);
    }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Message);
        writer.WriteByte(Position);
    }
}