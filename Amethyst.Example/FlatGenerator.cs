using Amethyst.Abstractions.Worlds;

namespace Amethyst.Example;

/// <summary>
/// Generates a superflat-like world: bedrock, three layers of dirt and grass on top.
/// </summary>
internal sealed class FlatGenerator : IGenerator
{
    public const int Height = 5;

    public void Generate(IWorld world, IChunk chunk, int x, int z)
    {
        for (var column = 0; column < 16; column++)
        {
            for (var row = 0; row < 16; row++)
            {
                chunk[column, 0, row] = Blocks.Bedrock;

                for (var y = 1; y < Height - 1; y++)
                {
                    chunk[column, y, row] = Blocks.Dirt;
                }

                chunk[column, Height - 1, row] = Blocks.GrassBlock;
            }
        }
    }
}
