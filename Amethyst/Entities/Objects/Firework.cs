using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class Firework(IWorld world) : Spawnable(world), IFirework
{
    public Item Item { get; set; }
}