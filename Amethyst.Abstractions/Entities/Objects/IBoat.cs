namespace Amethyst.Abstractions.Entities.Objects;

public interface IBoat : ISpawnable
{
    /// <summary>
    /// The ticks left for the boat to shake after being hit.
    /// </summary>
    public int TimeSinceHit { get; set; }

    /// <summary>
    /// The direction the boat shakes in, either 1 or -1.
    /// </summary>
    public int ForwardDirection { get; set; }

    /// <summary>
    /// The damage taken, which makes the boat shake harder.
    /// </summary>
    public float Damage { get; set; }
}