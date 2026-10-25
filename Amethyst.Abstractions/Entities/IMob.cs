namespace Amethyst.Abstractions.Entities;

public interface IMob : ILiving
{
    /// <summary>
    /// Whether the client animates the entity on its own, like turning its head.
    /// </summary>
    public bool HasAi { get; set; }
}