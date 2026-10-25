namespace Amethyst.Abstractions.Entities.Mobs;

public interface ICreeper : IMob
{
    /// <summary>
    /// Whether the creeper is swelling up to explode.
    /// </summary>
    public bool IsFusing { get; set; }

    public bool IsPowered { get; set; }
}