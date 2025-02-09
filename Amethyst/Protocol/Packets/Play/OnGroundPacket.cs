using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

public sealed record OnGroundPacket(bool Value) : IIngoingPacket<OnGroundPacket>, IPublisher
{
    public static int Identifier => 3;

    public static OnGroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new OnGroundPacket(reader.ReadBoolean());
    }

    async Task IPublisher.PublishAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(player, new Moved(player.X, player.Y, player.Z, player.Yaw, player.Pitch, Value), cancellationToken).ConfigureAwait(false);
    }
}