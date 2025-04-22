using Amethyst.Abstractions.Entities;
using Amethyst.Entities.Players;
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
        // To throw the item, or to move it.
        if (item.Type is -1)
        {
            return;
        }

        // Inventories are not fully implemented yet...
        var temporary = index - 36;

        client.Player!.Inventory.Slots[temporary] = item;
        eventDispatcher.Dispatch(new Inventory(), client.Player);
    }
}