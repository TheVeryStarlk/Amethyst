using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
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

    async Task IDispatchable.DispatchAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        var moved = new Moved(player.Location, Yaw, Pitch, OnGround);
        await eventDispatcher.DispatchAsync(player, moved, cancellationToken).ConfigureAwait(false);
    }
}