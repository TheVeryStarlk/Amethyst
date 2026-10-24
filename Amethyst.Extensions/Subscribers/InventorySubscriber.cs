using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Tracks the inventory changes players make into <c>player.Inventory</c>, only creative mode is supported.
/// </summary>
/// <remarks>
/// Subscribers registered before this one see the inventory before the change,
/// use the data of <see cref="Selected"/> and <see cref="SlotChanged"/> to react to the change itself.
/// </remarks>
public sealed class InventorySubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Selected>((source, selected) => source.Inventory.Track(selected.Slot));
            consumer.On<SlotChanged>((source, changed) => source.Inventory.Track(changed.Slot, changed.Item));
        });
    }
}
