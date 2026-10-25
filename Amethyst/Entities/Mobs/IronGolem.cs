using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class IronGolem(IWorld world) : Mob(world, 100), IIronGolem
{
    public bool IsPlayerCreated { get; set; }
}