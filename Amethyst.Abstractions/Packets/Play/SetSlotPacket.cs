using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class SetSlotPacket(short slot, Item item) : IOutgoingPacket
{
    public int Identifier => 47;

    public short Slot => slot;

    public Item Item => item;
}