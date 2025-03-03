using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region(World world, IGenerator generator)
{
    private readonly Dictionary<long, Chunk> chunks = [];

    public Block GetBlock(Position position)
    {
        return GetChunk(position.X, position.Z).GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        GetChunk(position.X, position.Z).SetBlock(block, position);
    }

    public byte GetSkyLight(Position position)
    {
        return GetChunk(position.X, position.Z).GetSkyLight(position);
    }

    public void SetSkyLight(byte value, Position position)
    {
        GetChunk(position.X, position.Z).SetSkyLight(value, position);
    }

    public Chunk GetChunk(int x, int z)
    {
        x >>= 4;
        z >>= 4;

        var value = NumericHelper.Encode(x, z);

        if (chunks.TryGetValue(value, out var chunk))
        {
            return chunk;
        }

        chunk = new Chunk(x, z);

        chunks[value] = chunk;
        generator.Generate(world, chunk);

        return chunk;
    }
}