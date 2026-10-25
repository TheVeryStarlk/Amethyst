namespace Amethyst.Abstractions.Entities.Objects;

/// <remarks>Clients only read the potion when the entity is spawned.</remarks>
public interface IThrownPotion : ISpawnable
{
    /// <summary>
    /// The durability of the potion item, which decides its color.
    /// </summary>
    public short Potion { get; set; }
}