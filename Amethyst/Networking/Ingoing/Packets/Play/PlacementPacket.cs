using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Networking.Ingoing.Packets.Play;

internal sealed class PlacementPacket(Position position, BlockFace face, Item item) : IIngoingPacket<PlacementPacket>
{
    public static int Identifier => 8;

    public Position Position => position;

    public BlockFace Face => face;

    public Item Item => item;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        var position = reader.ReadPosition();
        var face = (BlockFace) reader.ReadByte();
        var type = reader.ReadShort();

        if (type is -1)
        {
            return new PlacementPacket(position, face, new Item(-1, 0, 0));
        }

        var amount = reader.ReadByte();
        var durability = reader.ReadShort();

        var remaining = span[reader.Position..];

        if (remaining[0] is Tag.End)
        {
            return new PlacementPacket(position, face, new Item(type, amount, durability));
        }

        if (!TagSerializer.TryParse<CompoundTag>(remaining, out var tag))
        {
            throw new InvalidOperationException("Invalid NBT.");
        }

        return new PlacementPacket(position, face, new Item(type, amount, durability, tag));
    }
}