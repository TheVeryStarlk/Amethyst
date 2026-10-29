using System.Diagnostics;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Play;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class InventoryProcessor(Dispatcher dispatcher) : IProcessor<HeldItemChangePacket>, IProcessor<InventoryActionPacket>
{
    public void Process(Client client, HeldItemChangePacket packet)
    {
        Debug.Assert(client.Player is not null);

        if (packet.Index is < 0 or > 8)
        {
            return;
        }

        dispatcher.Dispatch(client.Player, new Selected(packet.Index));
    }

    public void Process(Client client, InventoryActionPacket packet)
    {
        Debug.Assert(client.Player is not null);

        // A slot of -1 means the item was dropped outside the window, which is not handled yet.
        if (packet.Index is < 0 or > 44)
        {
            return;
        }

        dispatcher.Dispatch(client.Player, new SlotChanged(packet.Index, packet.Item));
    }
}