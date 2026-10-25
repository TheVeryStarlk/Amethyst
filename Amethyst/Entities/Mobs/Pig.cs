using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Pig(IWorld world) : Ageable(world, 10), IPig
{
    public bool HasSaddle { get; set; }
}