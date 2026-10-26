using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal class Zombie(IWorld world) : Mob(world, 20), IZombie
{
    public bool IsChild { get; set; }

    public bool IsVillager { get; set; }

    public bool IsConverting { get; set; }
}