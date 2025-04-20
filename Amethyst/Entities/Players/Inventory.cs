using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Entities.Players;

internal sealed class Inventory : IInventory
{
    public Item[] Slots { get; } = new Item[9];

    public Item Holding { get; set; }
}