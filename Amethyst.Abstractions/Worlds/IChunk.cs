using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Abstractions.Worlds;

public interface IChunk
{
    public int X { get; }

    public int Z { get; }

    public Block this[int x, int y, int z] { get; set; }

    public Biome this[int x, int z] { get; set; }

    // Should this be public API?
    public SingleChunkPacket Build();
}