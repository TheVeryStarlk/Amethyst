namespace Amethyst.Abstractions.Entities.Objects;

public interface IArrow : ISpawnable
{
    public IEntity? Shooter { get; set; }

    /// <summary>
    /// Whether the arrow leaves a trail of critical hit particles.
    /// </summary>
    public bool IsCritical { get; set; }
}