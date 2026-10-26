using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Creeper(IWorld world) : Mob(world, 20), ICreeper
{
    public bool IsFusing { get; set; }

    public bool IsPowered { get; set; }
}