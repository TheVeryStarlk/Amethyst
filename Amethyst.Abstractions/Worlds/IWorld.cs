namespace Amethyst.Abstractions.Worlds;

public interface IWorld
{
    public string Name { get; }

    public WorldType Type { get; }

    public Dimension Dimension { get; }

    public Difficulty Difficulty { get; }

    public IGenerator Generator { get; }

    public Block this[int x, int y, int z] { get; set; }

    /// <summary>
    /// Returns a <see cref="IChunk"/>.
    /// </summary>
    /// <param name="x">The <see cref="IChunk"/>'s X position.</param>
    /// <param name="z">The <see cref="IChunk"/>'s Z position.</param>
    /// <remarks>
    /// The returned <see cref="IChunk"/> is not generated. Use <see cref="Generator"/> to generate it.
    /// </remarks>
    public IChunk this[int x, int z] { get; }
}