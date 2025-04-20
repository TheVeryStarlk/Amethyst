using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class HeldItemChangePacket(short index) : IIngoingPacket<HeldItemChangePacket>, IProcessor
{
    public static int Identifier => 9;

    public static HeldItemChangePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new HeldItemChangePacket(reader.ReadShort());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        // Why is it even a short?
        if (index is < 0 or >= 8)
        {
            return;
        }

        client.Player!.Inventory.Holding = client.Player.Inventory.Slots[index];
    }
}