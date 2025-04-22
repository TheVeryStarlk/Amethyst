namespace Amethyst.Abstractions.Entities.Player;

public interface IInventory
{
    public Item[] Slots { get; }

    public Item Holding { get; }
}