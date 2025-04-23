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
        get => Section(y)[x, y, z];
        set => Section(y)[x, y, z] = value;
    }

    public Biome this[int x, int z]
    {
        get => biomes[NumericUtility.AsIndex(x, z)];
        set => biomes[NumericUtility.AsIndex(x, z)] = value;
    }

    public SingleChunkPacket Build()
    {
        var parts = sections.OfType<Section>().Select(section => section.Build()).ToArray();

        // Use array pools for this.
        var final = Enumerable.Repeat<byte>(255, (8192 + 2048 + 2048) * parts.Length + 256).ToArray();

        var offset = 0;

        foreach (var part in parts)
        {
            part.CopyTo(final, offset);
            offset += 8192;
        }

        return new SingleChunkPacket(horizontal, vertical, final, (ushort) ((1 << parts.Length) - 1));
    }

    private Section Section(int y)
    {
        return sections[y.ToChunk()] ?? (sections[y.ToChunk()] = new Section());
    }
}