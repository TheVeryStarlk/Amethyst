using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region : IRegion
{
    public required int X { get; init; }

    public required int Z { get; init; }

    public IEnumerable<IChunk> Chunks => chunks;

    private readonly List<Chunk> chunks = [];

    public Chunk GetChunk(int x, int z)
    {
        x >>= 4;
        z >>= 4;

        var chunk = chunks.FirstOrDefault(chunk => chunk.X == x && chunk.Z == z);

        if (chunk is not null)
        {
            return chunk;
        }

        chunk = new Chunk(x, z);
        chunks.Add(chunk);

        return chunk;
    }

    public void SetChunk(Chunk chunk)
    {
        chunks.Add(chunk);
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