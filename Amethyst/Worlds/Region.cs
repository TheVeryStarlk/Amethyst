using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class Region(IGenerator generator)
{
    private readonly Dictionary<long, Chunk> chunks = [];

    public Block this[int x, int y, int z]
    {
        get => this[x.ToChunk(), z.ToChunk()][x, y, z];
        set => this[x.ToChunk(), z.ToChunk()][x, y, z] = value;
    }

    public Chunk this[int x, int z]
    {
        get
        {
            var value = NumericUtility.Encode(x, z);

            if (chunks.TryGetValue(value, out var chunk))
            {
                return chunk;
            }

            chunk = new Chunk(x, z);

            chunks[value] = chunk;
            generator.Generate(chunk);

            return chunk;
        }
    }
}