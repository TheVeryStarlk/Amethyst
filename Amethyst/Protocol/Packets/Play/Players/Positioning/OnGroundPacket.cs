using Amethyst.Abstractions.Eventing.Sources.Players;
using Amethyst.Abstractions.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players.Positioning;

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