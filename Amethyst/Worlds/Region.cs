using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region(IGenerator generator)
{
    private readonly Dictionary<long, Chunk> chunks = [];

    public Block GetBlock(Position position)
    {
        return GetChunk(position.ToChunk()).GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        GetChunk(position.ToChunk()).SetBlock(block, position);
    }

    public byte GetSkyLight(Position position)
    {
        return GetChunk(position.ToChunk()).GetSkyLight(position);
    }

    public void SetSkyLight(byte value, Position position)
    {
        GetChunk(position.ToChunk()).SetSkyLight(value, position);
    }

    public Chunk GetChunk(Position position)
    {
        var value = NumericsHelper.Encode(position.X, position.Z);

        if (chunks.TryGetValue(value, out var chunk))
        {
            return chunk;
        }

        chunk = new Chunk();

        chunks[value] = chunk;
        generator.Generate(chunk);

        return chunk;
    }
}