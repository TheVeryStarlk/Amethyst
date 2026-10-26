namespace Amethyst.Abstractions.Entities.Objects;

public interface IWitherSkull : IFireball
{
    /// <summary>
    /// Whether the skull is blue, like the ones withers shoot rarely.
    /// </summary>
    public bool IsCharged { get; set; }
}