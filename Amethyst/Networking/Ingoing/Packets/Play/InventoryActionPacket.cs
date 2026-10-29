using Amethyst.Abstractions.Entities;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Networking.Ingoing.Packets.Play;

internal sealed class InventoryActionPacket(short index, Item item) : IIngoingPacket<InventoryActionPacket>
{
    public static int Identifier => 16;

    public short Index => index;

    public Item Item => item;

    public static InventoryActionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        var index = reader.ReadShort();
        var type = reader.ReadShort();

        if (type is -1)
        {
            return new InventoryActionPacket(index, new Item(-1, 0, 0));
        }

        var amount = reader.ReadByte();
        var durability = reader.ReadShort();
        var remaining = span[reader.Position..];

        if (remaining[0] is Tag.End)
        {
            return new InventoryActionPacket(index, new Item(type, amount, durability));
        }

        if (!TagSerializer.TryParse<CompoundTag>(remaining, out var tag))
        {
            throw new InvalidOperationException("Invalid NBT.");
        }

        return new InventoryActionPacket(index, new Item(type, amount, durability, tag));
    }
}