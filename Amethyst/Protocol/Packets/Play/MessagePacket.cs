using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play;

internal sealed record MessagePacket(string Message, byte Position) : IIngoingPacket<MessagePacket>, IOutgoingPacket
{
    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => 2;

    public int Length => Variable.GetByteCount(Message) + sizeof(byte);

    public static MessagePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new MessagePacket(reader.ReadVariableString(), 0);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Message).WriteByte(Position);
    }
}