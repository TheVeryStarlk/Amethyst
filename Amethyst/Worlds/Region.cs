using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region(int x, int z, IGenerator generator) : IRegion
{
    public (int X, int Z) Position => (x, z);

    public IEnumerable<IChunk> Chunks => chunks;

    private readonly List<Chunk> chunks = [];

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
        foreach (var existing in chunks)
        {
            if (existing.Position == (position.X, position.Z))
            {
                return existing;
            }
        }

        var chunk = new Chunk(position.X, position.Z);

        generator.Generate(chunk);
        chunks.Add(chunk);

        return chunk;
    }
}