namespace Amethyst.Abstractions.Entities.Objects;

public interface IItemFrame : IHanging
{
    public Item Item { get; set; }

    /// <summary>
    /// The rotation of the item in steps of 45 degrees, from 0 to 7.
    /// </summary>
    public byte Rotation { get; set; }
}