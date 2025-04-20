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
    }
}