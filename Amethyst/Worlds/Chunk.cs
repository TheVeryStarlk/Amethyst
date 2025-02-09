namespace Amethyst.Worlds;

internal sealed record Chunk(int X, int Z)
{
    private readonly Section?[] sections = new Section[16];

    public Block GetBlock(long x, long y, long z)
    {
        var section = GetSection(x, y, z);
        return section.GetBlock(x % 16, y % 16, z % 16);
    }

    public void SetBlock(Block block, long x, long y, long z)
    {
        var section = GetSection(x, y, z);
        section.SetBlock(block, x % 16, y % 16, z % 16);
    }

    public byte GetSkyLight(long x, long y, long z)
    {
        var section = GetSection(x, y, z);
        return section.GetSkyLight(x % 16, y % 16, z % 16);
    }

    public void SetSkyLight(byte value, long x, long y, long z)
    {
        var section = GetSection(x, y, z);
        section.SetSkyLight(value, x % 16, y % 16, z % 16);
    }

    public (byte[] Buffer, ushort Bitmask) Build()
    {
        var serializedSections = sections
            .Where(section => section is not null)
            .Select(section => section!.Build())
            .ToArray();

        var payload = new List<byte>( /*12288 * serializedSections.Length + 256 */);

        // Each of blocks, blocks light and skylight must be added separately.
        foreach (var section in serializedSections)
        {
            payload.AddRange(section.Blocks);
        }

        foreach (var section in serializedSections)
        {
            payload.AddRange(section.BlocksLight);
        }

        foreach (var section in serializedSections)
        {
            payload.AddRange(section.SkyLight);
        }

        // Biomes.
        payload.AddRange(new byte[256]);

        return (payload.ToArray(), (ushort) ((1 << serializedSections.Length) - 1));
    }

    private Section GetSection(long x, long y, long z)
    {
        var index = y >> 4;
        return sections[index] ?? (sections[index] = new Section());
    }
}