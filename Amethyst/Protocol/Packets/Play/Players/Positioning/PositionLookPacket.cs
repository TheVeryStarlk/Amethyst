using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players.Positioning;

public sealed record PositionLookPacket(Location Location, float Yaw, float Pitch, bool OnGround)
    : IIngoingPacket<PositionLookPacket>, IOutgoingPacket, IDispatchable
{
    public static int Identifier => 6;

    int IOutgoingPacket.Identifier => 8;

    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    public static PositionLookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionLookPacket(
            new Location(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadBoolean());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(Location.X)
            .WriteDouble(Location.Y)
            .WriteDouble(Location.Z)
            .WriteFloat(Yaw)
            .WriteFloat(Pitch)
            .WriteBoolean(false);
    }

    void IDispatchable.Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        var moved = new Moved(Location, Yaw, Pitch, OnGround);
        eventDispatcher.Dispatch(player, moved);

        player.Location = Location;
        player.Yaw = Yaw;
        player.Pitch = Pitch;
        player.OnGround = OnGround;
    }
}