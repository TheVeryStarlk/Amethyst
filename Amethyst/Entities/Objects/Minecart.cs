using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal abstract class Minecart(IWorld world) : Spawnable(world), IMinecart
{
    public int ShakingPower { get; set; }

    public int ShakingDirection { get; set; } = 1;

    public float Damage { get; set; }

    public Block? DisplayedBlock { get; set; }

    public int DisplayedOffset { get; set; } = 6;
}