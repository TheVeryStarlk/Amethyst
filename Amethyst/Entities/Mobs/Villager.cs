using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Villager(IWorld world) : Ageable(world, 20), IVillager
{
    public Profession Profession { get; set; }
}