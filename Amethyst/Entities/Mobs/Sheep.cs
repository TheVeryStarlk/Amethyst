using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Sheep(IWorld world) : Ageable(world, 8), ISheep
{
    public DyeColor Color { get; set; }

    public bool IsSheared { get; set; }
}