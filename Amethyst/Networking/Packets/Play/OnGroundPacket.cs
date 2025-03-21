using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class OnGroundPacket(bool onGround) : IIngoingPacket<OnGroundPacket>, IProcessor
{
    public static int Identifier => 3;

    public static OnGroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new OnGroundPacket(reader.ReadBoolean());
    }

    public void Process(Player player, EventDispatcher eventDispatcher)
    {
        player.OnGround = onGround;
    }
}