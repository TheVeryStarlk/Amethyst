using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Ghast(IWorld world) : Mob(world, 10), IGhast
{
    public bool IsAttacking { get; set; }
}