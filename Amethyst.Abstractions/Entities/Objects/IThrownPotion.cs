namespace Amethyst.Abstractions.Entities.Objects;

public interface IThrownPotion : ISpawnable
{
    /// <summary>
    /// The durability of the potion item, which decides its color.
    /// </summary>
    public short Potion { get; set; }
}