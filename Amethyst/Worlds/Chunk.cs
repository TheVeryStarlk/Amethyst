using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed record Chunk(int X, int Z) : IChunk
{
    private readonly Section?[] sections = new Section[16];
    private readonly Biome[] biomes = new Biome[256];

    public Block this[int x, int y, int z]
    {
        get => GetSection(y)[x, y, z];
        set => GetSection(y)[x, y, z] = value;
    }

    public Biome this[int x, int z]
    {
        get => biomes[AsIndex(x, z)];
        set => biomes[AsIndex(x, z)] = value;
    }

    public SingleChunkPacket Build()
    {
        var parts = sections.OfType<Section>().Select(section => section.Build()).ToArray();

        var final = Enumerable.Repeat<byte>(255, 12288 * parts.Length + 256).ToArray();
        var index = 0;

        foreach (var part in parts)
        {
            part.CopyTo(final, index);
            index += 8192;
        }

        return new SingleChunkPacket(X, Z, final, (ushort) ((1 << parts.Length) - 1));
    }

    private static int AsIndex(int x, int z)
    {
        return (z & 0xF) * 16 + (x & 0xF);
    }

    private Section GetSection(int y)
    {
        var index = y >> 4;
        return sections[index] ?? (sections[index] = new Section());
    }
}