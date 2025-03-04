using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Protocol;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

internal sealed record PlacementPacket(Position Position, byte Face, Item Item) : IIngoingPacket<PlacementPacket>, IDispatchable
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        var position = Position.From(reader.ReadLong());
        var face = reader.ReadByte();

        var item = new Item(reader.ReadShort());

        if (item.Type <= 0)
        {
            return new PlacementPacket(position, face, item);
        }

        item.Amount = reader.ReadByte();
        item.Durability = reader.ReadShort();

        return new PlacementPacket(position, face, item);
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        if (Face is 255)
        {
            return;
        }

        eventDispatcher.Dispatch(player, new Placing(Position, (BlockFace) Face, Item));
    }
}