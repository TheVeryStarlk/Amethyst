namespace Amethyst.Abstractions.Entities;

public interface ISpawnable : IEntity
{
    public new Position Position { get; set; }

    public new float Yaw { get; set; }

    public new float Pitch { get; set; }

    public new bool Ground { get; set; }

    public Velocity Velocity { get; set; }

    public bool IsOnFire { get; set; }

    public bool IsCrouched { get; set; }

    public bool IsSprinting { get; set; }

    /// <summary>
    /// Whether the entity is eating, drinking or blocking.
    /// </summary>
    public bool IsEating { get; set; }

    public bool IsInvisible { get; set; }

    /// <summary>
    /// The air left while underwater, from 300 down to 0.
    /// </summary>
    public short Air { get; set; }

    public string? CustomName { get; set; }

    /// <summary>
    /// Whether the <see cref="CustomName"/> is shown even when not looking at the entity.
    /// </summary>
    public bool IsCustomNameVisible { get; set; }

    public bool IsSilent { get; set; }
}