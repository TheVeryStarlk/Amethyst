using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Witch(IWorld world) : Mob(world, 26), IWitch
{
    public bool IsAggressive { get; set; }
}