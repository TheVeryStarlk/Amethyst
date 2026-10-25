using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class DroppedItem(IWorld world) : Spawnable(world), IDroppedItem
{
    public Item Item { get; set; }
}