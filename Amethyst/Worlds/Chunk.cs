using Amethyst.Components.Worlds;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Worlds;

internal sealed class Chunk(int x, int z) : IChunk
{
    public int X { get; } = x;

    public int Z { get; } = z;

    private readonly Section?[] sections = new Section[16];

    public Block GetBlock(Position position)
    {
        var section = GetSection(position.Y);
        return section.GetBlock(position % 16);
    }

    public void SetBlock(Block block, Position position)
    {
        var section = GetSection(position.Y);
        section.SetBlock(block, position % 16);
    }

    public byte GetSkyLight(Position position)
    {
        var section = GetSection(position.Y);
        return section.GetSkyLight(position % 16);
    }

    public void SetSkyLight(byte value, Position position)
    {
        var section = GetSection(position.Y);
        section.SetSkyLight(value, position % 16);
    }

    public SingleChunkPacket Build()
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

        return new SingleChunkPacket(X, Z, list.ToArray(), (ushort) ((1 << serializedSections.Length) - 1));
    }

    private Section GetSection(int y)
    {
        var index = y >> 4;
        return sections[index] ?? (sections[index] = new Section());
    }
}