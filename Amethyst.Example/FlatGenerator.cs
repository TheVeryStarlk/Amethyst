using Amethyst.Abstractions.Worlds;

namespace Amethyst.Example;

internal sealed class FlatGenerator : IGenerator
{
    private readonly Biome biome = (Biome) Random.Shared.Next(120);

    public void Generate(IChunk chunk)
    {
        for (var x = 0; x < 16; x++)
        {
            for (var z = 0; z < 16; z++)
            {
                chunk.SetBiome(biome, x, z);

                chunk.SetBlock(new Block(7), new Position(x, 0, z));
                chunk.SetBlock(new Block(3), new Position(x, 1, z));
                chunk.SetBlock(new Block(3), new Position(x, 2, z));
                chunk.SetBlock(new Block(2), new Position(x, 3, z));

                var topping = Random.Shared.Next(byte.MaxValue) > sbyte.MaxValue ? new Block(31, 1) : new Block();
                chunk.SetBlock(topping, new Position(x, 4, z));
            }
        }
    }
}