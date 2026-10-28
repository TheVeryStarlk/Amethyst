using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

// Different naming so that it does not hide the variable names used inside the class.
internal sealed class Chunk(int horizontal, int vertical) : IChunk
{
    private readonly Section?[] sections = new Section[16];
    private readonly Biome[] biomes = new Biome[256];

    public Block this[int x, int y, int z]
    {
        get => sections[y.ToChunk()]?[x, y, z] ?? Blocks.Air;
        set
        {
            var section = sections[y.ToChunk()] ?? (sections[y.ToChunk()] = new Section());
            section[x, y, z] = value;
        }
    }

    public Biome this[int x, int z]
    {
        get => biomes[NumericUtility.AsIndex(x, z)];
        set => biomes[NumericUtility.AsIndex(x, z)] = value;
    }

    public SingleChunkPacket Build()
    {
        var parts = new byte[sections.Length][];
        var count = 0;
        var bitmask = 0;

        // Sections can have gaps between them, the bitmask tells the client which heights are present.
        for (var index = 0; index < sections.Length; index++)
        {
            if (sections[index] is not { } section)
            {
                continue;
            }

            parts[count++] = section.Build();
            bitmask |= 1 << index;
        }

        // Use array pools for this.
        var final = Enumerable.Repeat<byte>(255, (8192 + 2048 + 2048) * count + 256).ToArray();

        var offset = 0;

        foreach (var part in parts.AsSpan(0, count))
        {
            part.CopyTo(final, offset);
            offset += 8192;
        }

        for (var index = 0; index < biomes.Length; index++)
        {
            final[final.Length - biomes.Length + index] = (byte) biomes[index];
        }

        return new SingleChunkPacket(horizontal, vertical, final, (ushort) bitmask);
    }
}