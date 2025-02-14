using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
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

    async Task IDispatchable.DispatchAsync(Player player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        player.OnGround = Value;

        var moved = new Moved(player.Location, player.Yaw, player.Pitch, Value);
        await eventDispatcher.DispatchAsync(player, moved, cancellationToken).ConfigureAwait(false);
    }
}