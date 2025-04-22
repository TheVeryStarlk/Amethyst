namespace Amethyst.Abstractions.Worlds;

/// <summary>
/// Represents an <see cref="IChunk"/> generator.
/// </summary>
public interface IGenerator
{
    /// <summary>
    /// Generates a singular <see cref="IChunk"/>.
    /// </summary>
    /// <param name="world">The <see cref="IWorld"/> the current <see cref="IChunk"/> resides in.</param>
    /// <param name="chunk">The <see cref="IChunk"/> instance to generate.</param>
    /// <param name="x">The <see cref="IChunk"/>'s X position.</param>
    /// <param name="z">The <see cref="IChunk"/>'s Z position.</param>
    public void Generate(IWorld world, IChunk chunk, int x, int z);
}