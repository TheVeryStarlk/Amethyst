using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Chunk(int x, int z) : IChunk
{
    public (int X, int Z) Position => (x, z);

    private readonly Section?[] sections = new Section[16];

    public Block GetBlock(Position position)
    {
        return GetSection(position.Y).GetBlock(position.ToSection());
    }

    public void SetBlock(Block block, Position position)
    {
        GetSection(position.Y).SetBlock(block, position.ToSection());
    }

    public byte GetSkyLight(Position position)
    {
        return GetSection(position.Y).GetSkyLight(position.ToSection());
    }

    public void SetSkyLight(byte value, Position position)
    {
        GetSection(position.Y).SetSkyLight(value, position.ToSection());
    }

    public (byte[] Chunk, ushort Bitmask) Build()
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