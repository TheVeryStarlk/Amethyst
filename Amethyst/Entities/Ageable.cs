using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal abstract class Ageable(IWorld world, float health) : Mob(world, health), IAgeable
{
    public bool IsChild { get; set; }
}