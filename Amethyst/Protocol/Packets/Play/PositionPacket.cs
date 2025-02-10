using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

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

    async Task IDispatchable.DispatchAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        var moved = new Moved(Location, player.Yaw, player.Pitch, OnGround);
        await eventDispatcher.DispatchAsync(player, moved, cancellationToken).ConfigureAwait(false);
    }
}