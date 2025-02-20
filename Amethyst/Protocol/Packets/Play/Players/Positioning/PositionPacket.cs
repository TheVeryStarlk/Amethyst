using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players.Positioning;

public sealed record PositionPacket(Location Location, bool OnGround) : IIngoingPacket<PositionPacket>, IDispatchable
{
    public static int Identifier => 4;

    public static PositionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionPacket(
            new Location(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadBoolean());
    }

    void IDispatchable.Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        var moved = new Moved(Location, player.Yaw, player.Pitch, OnGround);
        eventDispatcher.Dispatch(player, moved);

        player.Location = Location;
        player.OnGround = OnGround;
    }
}