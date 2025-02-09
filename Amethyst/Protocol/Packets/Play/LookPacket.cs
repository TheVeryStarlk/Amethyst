using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

public sealed record LookPacket(float Yaw, float Pitch, bool OnGround) : IIngoingPacket<LookPacket>, IPublisher
{
    public static int Identifier => 5;

    public static LookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LookPacket(reader.ReadFloat(), reader.ReadFloat(), reader.ReadBoolean());
    }

    async Task IPublisher.PublishAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(player, new Moved(player.X, player.Y, player.Z, Yaw, Pitch, OnGround), cancellationToken);
    }
}