using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Levels;

internal sealed class Region(long x, long z)
{
    public (long X, long Z) Position { get; } = (x, z);

    private readonly List<Chunk> chunks = [];

    public Chunk GetChunk(Position position)
    {
        var (x, z) = (position.X >> 4, position.Z >> 4);
        var chunk = chunks.FirstOrDefault(chunk => chunk.Position == (x, z));

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
        var chunk = GetChunk(position);
        return chunk.GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        var chunk = GetChunk(position);
        chunk.SetBlock(block, position);
    }

    public byte GetSkyLight(Position position)
    {
        var chunk = GetChunk(position);
        return chunk.GetSkyLight(position);
    }

    public void SetSkyLight(byte value, Position position)
    {
        var chunk = GetChunk(position);
        chunk.SetSkyLight(value, position);
    }
}