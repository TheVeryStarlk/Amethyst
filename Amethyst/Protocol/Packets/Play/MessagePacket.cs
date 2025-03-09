using Amethyst.Abstractions.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Players;

namespace Amethyst.Protocol.Packets.Play;

public sealed record MessagePacket(string Message, byte Position) : IIngoingPacket<MessagePacket>, IOutgoingPacket, IDispatchable
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

    void IDispatchable.Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        var sent = new Sent(Message);
        eventDispatcher.Dispatch(player, sent);
    }
}