namespace Amethyst.Abstractions.Entities.Player;

public interface IInventoryViewer
{
    public IInventory For(IPlayer player);
}