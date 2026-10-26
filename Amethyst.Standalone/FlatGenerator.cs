using Amethyst.Abstractions.Worlds;

namespace Amethyst.Standalone;

/// <summary>
/// Generates a bedrock layer, two dirt layers and a grass layer on top.
/// </summary>
internal sealed class FlatGenerator : IGenerator
{
    public static FlatGenerator Instance { get; } = new();

    /// <summary>
    /// The first air layer, where players stand.
    /// </summary>
    public const int Height = 4;

    private FlatGenerator()
    {
        // No need to create an instance of it.
    }

    public void Generate(IWorld world, IChunk chunk, int x, int z)
    {
        for (var localX = 0; localX < 16; localX++)
        {
            for (var localZ = 0; localZ < 16; localZ++)
            {
                chunk[localX, 0, localZ] = Blocks.Bedrock;
                chunk[localX, 1, localZ] = Blocks.Dirt;
                chunk[localX, 2, localZ] = Blocks.Dirt;
                chunk[localX, 3, localZ] = Blocks.GrassBlock;
            }
        }
    }
}