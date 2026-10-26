using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Blaze(IWorld world) : Mob(world, 20), IBlaze
{
    public bool IsCharging { get; set; }
}