namespace Amethyst.Abstractions.Entities.Objects;

public interface IExperienceOrb : ISpawnable
{
    /// <summary>
    /// The experience the orb gives, which also decides its size.
    /// </summary>
    public short Count { get; set; }
}