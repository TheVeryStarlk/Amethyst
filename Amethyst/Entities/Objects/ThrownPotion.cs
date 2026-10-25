using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class ThrownPotion(IWorld world) : Spawnable(world), IThrownPotion
{
    public short Potion { get; set; }
}