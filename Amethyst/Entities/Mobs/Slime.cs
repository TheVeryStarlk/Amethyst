using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

// The maximum health is the size squared.
internal class Slime(IWorld world) : Mob(world, 1), ISlime
{
    public byte Size { get; set; } = 1;
}