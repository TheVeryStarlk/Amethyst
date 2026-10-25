using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Bat(IWorld world) : Mob(world, 6), IBat
{
    public bool IsHanging { get; set; }
}