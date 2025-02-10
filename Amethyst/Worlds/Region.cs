using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region(int x, int z) : IRegion
{
    public int X { get; } = x;

    public int Z { get; } = z;

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

    public Block GetBlock(int x, int y, int z)
    {
        var chunk = GetChunk(x, z);
        return chunk.GetBlock(x, y, z);
    }

    public void SetBlock(Block block, int x, int y, int z)
    {
        var chunk = GetChunk(x, z);
        chunk.SetBlock(block, x, y, z);
    }

    public byte GetSkyLight(int x, int y, int z)
    {
        var chunk = GetChunk(x, z);
        return chunk.GetSkyLight(x, y, z);
    }

    public void SetSkyLight(byte value, int x, int y, int z)
    {
        var chunk = GetChunk(x, z);
        chunk.SetSkyLight(value, x, y, z);
    }
}