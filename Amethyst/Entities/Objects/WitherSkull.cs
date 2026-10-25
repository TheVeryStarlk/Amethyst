using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class WitherSkull(IWorld world) : Fireball(world), IWitherSkull
{
    public bool IsCharged { get; set; }
}