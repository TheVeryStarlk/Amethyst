using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities.Objects;

public interface IMinecart : ISpawnable
{
    /// <summary>
    /// The ticks left for the minecart to shake after being hit.
    /// </summary>
    public int ShakingPower { get; set; }

    /// <summary>
    /// The direction the minecart shakes in, either 1 or -1.
    /// </summary>
    public int ShakingDirection { get; set; }

    /// <summary>
    /// The damage taken, which makes the minecart shake harder.
    /// </summary>
    public float Damage { get; set; }

    /// <summary>
    /// The block shown inside the minecart instead of its usual one.
    /// </summary>
    public Block? DisplayedBlock { get; set; }

    /// <summary>
    /// How many pixels the <see cref="DisplayedBlock"/> is raised by.
    /// </summary>
    public int DisplayedOffset { get; set; }
}