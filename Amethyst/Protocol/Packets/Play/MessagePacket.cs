using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

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

    async Task IDispatchable.DispatchAsync(Player player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        var sent = new Sent(Message);
        await eventDispatcher.DispatchAsync(player, sent, cancellationToken).ConfigureAwait(false);
    }
}