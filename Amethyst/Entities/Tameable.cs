using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal abstract class Tameable(IWorld world, float health) : Ageable(world, health), ITameable
{
    public bool IsSitting { get; set; }

    public bool IsTamed { get; set; }

    public Guid? Owner { get; set; }
}