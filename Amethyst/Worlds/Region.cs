using Amethyst.Api.Components;
using Amethyst.Api.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region
{
    public required Position Position { get; init; }

    private readonly List<Chunk> chunks = [];

    public Block GetBlock(Position position)
    {
        var chunk = GetChunk(position, true);
        return chunk.GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        var chunk = GetChunk(position, true);
        chunk.SetBlock(block, position);
    }

    public byte GetSkyLight(Position position)
    {
        var chunk = GetChunk(position, true);
        return chunk.GetSkyLight(position);
    }

    public void SetSkyLight(byte value, Position position)
    {
        var chunk = GetChunk(position, true);
        chunk.SetSkyLight(value, position);
    }

    public Chunk GetChunk(Position position, bool shift = false)
    {
        position = shift ? position.ToChunkCoordinates() : position;
        var chunk = chunks.FirstOrDefault(chunk => chunk.Position == position);

        if (chunk is not null)
        {
            return chunk;
        }

        chunk = new Chunk
        {
            Position = position
        };

        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                chunk.SetBlock(new Block
                {
                    Type = 7
                }, new Position(x,0 ,z));
            }
        }

        chunks.Add(chunk);
        return chunk;
    }
}