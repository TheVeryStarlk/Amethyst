using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

/// <summary>
/// Represents an empty <see cref="IGenerator"/>.
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
    }
}