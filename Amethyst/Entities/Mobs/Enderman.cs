using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Enderman(IWorld world) : Mob(world, 40), IEnderman
{
    public Block Carried { get; set; }

    public bool IsScreaming { get; set; }
}