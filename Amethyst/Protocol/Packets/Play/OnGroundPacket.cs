using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

public sealed record OnGroundPacket(bool Value) : IIngoingPacket<OnGroundPacket>, IDispatchable
{
    public static int Identifier => 3;

    public static OnGroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new OnGroundPacket(reader.ReadBoolean());
    }

    void IDispatchable.Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        var moved = new Moved(player.Location, player.Yaw, player.Pitch, Value);
        eventDispatcher.Dispatch(player, moved);

        player.OnGround = Value;
    }
}