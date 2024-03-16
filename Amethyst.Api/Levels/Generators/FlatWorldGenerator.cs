using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Api.Levels.Generators;

public sealed class FlatWorldGenerator : IWorldGenerator
{
    public void GenerateChunk(IChunk chunk)
    {
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 4; y++)
            {
                for (var z = 0; z < 16; z++)
                {
                    var position = new Position(x, y, z);

                    var type = y switch
                    {
                        >= 1 and <= 2 => 3,
                        3 => 2,
                        _ => 7
                    };

                    chunk.SetBlock(new Block(type), position);
                }
            }
        }
    }
}