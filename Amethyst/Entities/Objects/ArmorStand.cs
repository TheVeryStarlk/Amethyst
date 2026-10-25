using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class ArmorStand(IWorld world) : Living(world, 20), IArmorStand
{
    public bool IsSmall { get; set; }

    public bool HasGravity { get; set; } = true;

    public bool HasArms { get; set; }

    public bool HasBasePlate { get; set; } = true;

    public bool IsMarker { get; set; }

    public EulerAngle Head { get; set; }

    public EulerAngle Body { get; set; }

    public EulerAngle LeftArm { get; set; } = new(-10, 0, -10);

    public EulerAngle RightArm { get; set; } = new(-15, 0, 10);

    public EulerAngle LeftLeg { get; set; } = new(-1, 0, -1);

    public EulerAngle RightLeg { get; set; } = new(1, 0, 1);
}