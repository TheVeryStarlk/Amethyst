using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Extensions;

/// <summary>
/// A player's inventory window, every change made through it is sent to the player.
/// </summary>
/// <remarks>Accessed through <c>player.Inventory</c>.</remarks>
public sealed class Inventory(IPlayer player)
{
    public const int Length = 45;

    // Where each section of the inventory window starts.
    public const int Output = 0;
    public const int Crafting = 1;
    public const int Armor = 5;
    public const int Main = 9;
    public const int Hotbar = 36;

    private readonly Item[] slots = new Item[Length];

    public Item this[int slot] => slots[slot];

    /// <remarks>Ranges from 0 to 8.</remarks>
    public short Selected { get; private set; }

    public Item Holding => slots[Hotbar + Selected];

    public void Set(int slot, Item item)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(slot, Length);

        slots[slot] = item;
        player.Client.Write(new SetSlotPacket((short) slot, item));
    }

    public void Select(int hotbar)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(hotbar);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hotbar, 8);

        Selected = (short) hotbar;
        player.Client.Write(new HeldSlotPacket((byte) hotbar));
    }

    public void Clear()
    {
        Array.Clear(slots);
        player.Client.Write(new WindowItemsPacket(slots.ToArray()));
    }

    // The client already knows about these changes, so they are not sent back.
    internal void Track(short slot, Item item)
    {
        slots[slot] = item;
    }

    internal void Track(short selected)
    {
        Selected = selected;
    }
}
