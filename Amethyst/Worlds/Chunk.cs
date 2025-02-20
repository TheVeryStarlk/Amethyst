using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class Chunk : IChunk
{
    private readonly Section?[] sections = new Section[16];
    private readonly byte[] biomes = new byte[256];

    public Block GetBlock(Position position)
    {
        return GetSection(position.Y).GetBlock(position.ToSection());
    }

    public void SetBlock(Block block, Position position)
    {
        GetSection(position.Y).SetBlock(block, position.ToSection());
    }

    public Biome GetBiome(int x, int z)
    {
        return (Biome) biomes[(z & 0xF) * 16 + (x & 0xF)];
    }

    public void SetBiome(Biome biome, int x, int z)
    {
        biomes[(z & 0xF) * 16 + (x & 0xF)] = (byte) biome;
    }

    public byte GetSkyLight(Position position)
    {
        return GetSection(position.Y).GetSkyLight(position.ToSection());
    }

    public void SetSkyLight(byte value, Position position)
    {
        GetSection(position.Y).SetSkyLight(value, position.ToSection());
    }

    public (byte[] Sections, ushort Bitmask) Build()
    {
        var serializedSections = sections
            .OfType<Section>()
            .Select(section => section.Build())
            .ToArray();

        var list = new List<byte>(12288 * serializedSections.Length + 256);

        // Each of blocks, blocks light and skylight must be added separately.
        foreach (var section in serializedSections)
        {
            list.AddRange(section.Blocks);
        }

        foreach (var section in serializedSections)
        {
            list.AddRange(section.BlocksLight);
        }

        foreach (var section in serializedSections)
        {
            list.AddRange(section.SkyLight);
        }

        list.AddRange(biomes);

        return (list.ToArray(), (ushort) ((1 << serializedSections.Length) - 1));
    }

    private Section GetSection(int y)
    {
        var index = y >> 4;
        return sections[index] ?? (sections[index] = new Section());
    }
}