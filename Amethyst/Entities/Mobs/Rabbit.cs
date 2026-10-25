using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Rabbit(IWorld world) : Ageable(world, 3), IRabbit
{
    public RabbitType Type { get; set; }
}