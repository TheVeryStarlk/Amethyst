using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region(Position where, IGenerator generator) : IRegion
{
    public Position Position => where;

    public IEnumerable<IChunk> Chunks => chunks;

    private readonly List<Chunk> chunks = [];

    public Chunk GetChunk(int x, int z)
    {
        var chunk = chunks.FirstOrDefault(chunk => (chunk.X, chunk.Z) == (x, z));

        if (chunk is not null)
        {
            return chunk;
        }

        chunk = new Chunk(x, z);
        generator.Generate(chunk);
        chunks.Add(chunk);

        return chunk;
    }

    public Block GetBlock(Position position)
    {
        var chunk = GetChunk(position.X, position.Z);
        return chunk.GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        var chunk = GetChunk(position.X, position.Z);
        chunk.SetBlock(block, position);
    }

    public byte GetSkyLight(Position position)
    {
        var chunk = GetChunk(position.X, position.Z);
        return chunk.GetSkyLight(position);
    }

    public void SetSkyLight(byte value, Position position)
    {
        var chunk = GetChunk(position.X, position.Z);
        chunk.SetSkyLight(value, position);
    }
}