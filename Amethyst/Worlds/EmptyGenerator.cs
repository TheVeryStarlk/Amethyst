using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

/// <summary>
/// Generates <see cref="Blocks.Air"/>-filled <see cref="IChunk"/>s.
/// </summary>
public sealed class EmptyGenerator : IGenerator
{
    /// <summary>
    /// A static instance of the <see cref="EmptyGenerator"/>.
    /// </summary>
    public static EmptyGenerator Instance { get; } = new();

    private EmptyGenerator()
    {
        // No need to create an instance of it.
    }

    public void Generate(IWorld world, IChunk chunk, int x, int z)
    {
        chunk[x, 0, z] = Blocks.Air;
    }
}