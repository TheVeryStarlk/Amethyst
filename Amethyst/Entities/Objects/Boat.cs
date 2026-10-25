using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class Boat(IWorld world) : Spawnable(world), IBoat
{
    public int TimeSinceHit { get; set; }

    public int ForwardDirection { get; set; } = 1;

    public float Damage { get; set; }
}