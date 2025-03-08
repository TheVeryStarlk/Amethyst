using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

public sealed record LookPacket(float Yaw, float Pitch, bool OnGround) : IIngoingPacket<LookPacket>, IDispatchable
{
    public static int Identifier => 5;

    public static LookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LookPacket(reader.ReadFloat(), reader.ReadFloat(), reader.ReadBoolean());
    }

    void IDispatchable.Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        var moved = new Moved(player.Location, Yaw, Pitch, OnGround);
        eventDispatcher.Dispatch(player, moved);

        player.Yaw = Yaw;
        player.Pitch = Pitch;
        player.OnGround = OnGround;
    }
}