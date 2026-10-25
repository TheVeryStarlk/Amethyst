namespace Amethyst.Abstractions.Entities.Objects;

public interface IArmorStand : ILiving
{
    public bool IsSmall { get; set; }

    public bool HasGravity { get; set; }

    public bool HasArms { get; set; }

    public bool HasBasePlate { get; set; }

    /// <summary>
    /// Whether the armor stand has a tiny hitbox and can not be interacted with.
    /// </summary>
    public bool IsMarker { get; set; }

    public EulerAngle Head { get; set; }

    public EulerAngle Body { get; set; }

    public EulerAngle LeftArm { get; set; }

    public EulerAngle RightArm { get; set; }

    public EulerAngle LeftLeg { get; set; }

    public EulerAngle RightLeg { get; set; }
}