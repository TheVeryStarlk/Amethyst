using Amethyst.Components.Entities;
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

    async Task IPublisher.PublishAsync(Packet packet, IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        var instance = packet.Create<PositionPacket>();
        var moved = new Moved(instance.X, instance.Y, instance.Z, player.Yaw, player.Pitch, instance.OnGround);

        await eventDispatcher.DispatchAsync(player, moved, cancellationToken).ConfigureAwait(false);
    }
}