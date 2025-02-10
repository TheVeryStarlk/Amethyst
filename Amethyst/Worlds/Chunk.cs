using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Chunk(int x, int z) : IChunk
{
    public int X { get; } = x;

    public int Z { get; } = z;

    private readonly Section?[] sections = new Section[16];

    public Block GetBlock(int x, int y, int z)
    {
        var section = GetSection(y);
        return section.GetBlock(x % 16, y % 16, z % 16);
    }

    public void SetBlock(Block block, int x, int y, int z)
    {
        var section = GetSection(y);
        section.SetBlock(block, x % 16, y % 16, z % 16);
    }

    public byte GetSkyLight(int x, int y, int z)
    {
        var section = GetSection(y);
        return section.GetSkyLight(x % 16, y % 16, z % 16);
    }

    public void SetSkyLight(byte value, int x, int y, int z)
    {
        var section = GetSection(y);
        section.SetSkyLight(value, x % 16, y % 16, z % 16);
    }

    public (byte[] Buffer, ushort Bitmask) Build()
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

        // Biomes.
        list.AddRange(new byte[256]);

        return (list.ToArray(), (ushort) ((1 << serializedSections.Length) - 1));
    }

    private Section GetSection(int y)
    {
        var index = y >> 4;
        return sections[index] ?? (sections[index] = new Section());
    }
}