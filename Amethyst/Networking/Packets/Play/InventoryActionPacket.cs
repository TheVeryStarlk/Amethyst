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

        var index = reader.ReadShort();
        var type = reader.ReadShort();

        if (type is -1)
        {
            return new InventoryActionPacket(index, new Item(-1, 0, 0));
        }

        var item = new Item(type, reader.ReadByte(), reader.ReadShort());

        if (reader.ReadBoolean())
        {
            throw new InvalidOperationException("Has NBT.");
        }

        return new InventoryActionPacket(index, item);
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
        eventDispatcher.Dispatch(client.Player, new Abstractions.Eventing.Player.Inventory());
    }
}
