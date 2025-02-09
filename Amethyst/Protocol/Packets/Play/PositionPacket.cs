using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

public sealed record PositionPacket(double X, double Y, double Z, bool OnGround) : IIngoingPacket<PositionPacket>, IPublisher
{
    public static int Identifier => 4;

    public static PositionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PositionPacket(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadBoolean());
    }

    async Task IPublisher.PublishAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(player, new Moved(X, Y, Z, player.Yaw, player.Pitch, OnGround), cancellationToken).ConfigureAwait(false);
    }
}