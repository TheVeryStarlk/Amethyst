namespace Amethyst.Abstractions.Entities.Mobs;

public interface ISlime : IMob
{
    /// <summary>
    /// The width in blocks, usually 1, 2 or 4.
    /// </summary>
    public byte Size { get; set; }
}