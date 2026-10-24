using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Eventing;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Networking.Packets.Play;

internal sealed class InventoryActionPacket(short index, Item item) : IIngoingPacket<InventoryActionPacket>, IProcessor
{
    public static int Identifier => 16;

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

        // Items without NBT only have a single end tag.
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

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        Debug.Assert(client.Player is not null);

        // A slot of -1 means the item was dropped outside the window, which is not handled yet.
        if (index is < 0 or > 44)
        {
            return;
        }

        eventDispatcher.Dispatch(client.Player, new SlotChanged(index, item));
    }
}
