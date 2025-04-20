using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class InventoryActionPacket(short index, Item item) : IIngoingPacket<InventoryActionPacket>, IProcessor
{
    public static int Identifier => 16;

    public static InventoryActionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new InventoryActionPacket(reader.ReadShort(), reader.ReadItem());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var temporary = index switch
        {
            36 => 0,
            37 => 1,
            38 => 2,
            39 => 3,
            40 => 4,
            41 => 5,
            42 => 6,
            43 => 7,
            44 => 8,
            _ => -1
        };

        // Inventories are not fully implemented yet...
        if (temporary is -1)
        {
            return;
        }

        // To throw the item, or to move it.
        if (item.Type is -1)
        {
            return;
        }

        client.Player!.Inventory.Slots[temporary] = item;
    }
}