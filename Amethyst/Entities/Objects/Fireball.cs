using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal abstract class Fireball(IWorld world) : Spawnable(world), IFireball
{
    public IEntity? Shooter { get; set; }
}