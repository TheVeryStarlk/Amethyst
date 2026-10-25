namespace Amethyst.Abstractions.Entities.Objects;

public interface IFirework : ISpawnable
{
    /// <summary>
    /// The firework item, its tag decides how the firework explodes.
    /// </summary>
    public Item Item { get; set; }
}