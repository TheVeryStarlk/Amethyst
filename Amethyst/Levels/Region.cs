using Amethyst.Api.Components;
using Amethyst.Api.Levels;
using Amethyst.Api.Levels.Blocks;
using Amethyst.Api.Levels.Generators;

namespace Amethyst.Levels;

internal sealed class Region(long x, long z, IWorldGenerator generator) : IRegion
{
    public (long X, long Z) Position { get; } = (x, z);

    public IEnumerable<IChunk> Chunks => chunks;

    private readonly List<Chunk> chunks = [];

    public Chunk GetChunk(Position position)
    {
        var (x, z) = (position.X, position.Z);
        var chunk = chunks.FirstOrDefault(chunk => chunk.Position == (x, z));

        if (chunk is not null)
        {
            return chunk;
        }

        chunk = new Chunk(x, z);
        generator.GenerateChunk(chunk);
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