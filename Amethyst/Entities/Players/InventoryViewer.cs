using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Entities.Players;

internal sealed class InventoryViewer : IInventoryViewer
{
    public IInventory For(IPlayer player)
    {
        // Any better way for this?
        if (player is Player instance)
        {
            return instance.Inventory;
        }

        throw new InvalidOperationException("Invalid player.");
    }
}