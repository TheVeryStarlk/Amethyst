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
        // Reading must not allocate, otherwise every empty section read would be sent to clients.
        get => sections[y.ToChunk()]?[x, y, z] ?? Blocks.Air;
        set => Section(y)[x, y, z] = value;
    }

    public Biome this[int x, int z]
    {
        get => biomes[NumericUtility.AsIndex(x, z)];
        set => biomes[NumericUtility.AsIndex(x, z)] = value;
    }

    public SingleChunkPacket Build()
    {
        var parts = new List<byte[]>();
        var bitmask = 0;

        // Sections can have gaps between them, the bitmask tells the client which heights are present.
        for (var index = 0; index < sections.Length; index++)
        {
            if (sections[index] is not { } section)
            {
                continue;
            }

            parts.Add(section.Build());
            bitmask |= 1 << index;
        }

        // Use array pools for this.
        var final = Enumerable.Repeat<byte>(255, (8192 + 2048 + 2048) * parts.Count + 256).ToArray();

        var offset = 0;

        foreach (var part in parts)
        {
            part.CopyTo(final, offset);
            offset += 8192;
        }

        return new SingleChunkPacket(horizontal, vertical, final, (ushort) bitmask);
    }

    private Section Section(int y)
    {
        return sections[y.ToChunk()] ?? (sections[y.ToChunk()] = new Section());
    }
}