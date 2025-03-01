using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Protocol;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players;

internal sealed record PlacementPacket(Position Position, BlockFace Face) : IIngoingPacket<PlacementPacket>, IDispatchable
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PlacementPacket(Position.From(reader.ReadLong()), (BlockFace) reader.ReadByte());
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(player, new Placing(Position, Face));
    }
}