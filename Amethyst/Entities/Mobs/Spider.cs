using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal class Spider(IWorld world, float health = 16) : Mob(world, health), ISpider
{
    public bool IsClimbing { get; set; }
}