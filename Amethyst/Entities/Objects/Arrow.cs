using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class Arrow(IWorld world) : Spawnable(world), IArrow
{
    public IEntity? Shooter { get; set; }

    public bool IsCritical { get; set; }
}