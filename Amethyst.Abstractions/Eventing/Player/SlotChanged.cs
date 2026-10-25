using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

/// <summary>
/// The player set a slot in their inventory, only sent by clients in creative mode.
/// </summary>
public sealed class SlotChanged(short slot, Item item) : IEvent<IPlayer>
{
    /// <remarks>Ranges from 0 to 44, following the player's inventory window layout.</remarks>
    public short Slot => slot;

    /// <remarks>Could be empty, see <see cref="Entities.Item.IsEmpty"/>.</remarks>
    public Item Item => item;
}