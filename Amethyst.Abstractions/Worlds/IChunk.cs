using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Abstractions.Worlds;

public interface IChunk
{
    public int X { get; }

    public int Z { get; }

    public Biome this[int x, int z] { get; set; }

    // Should this be public API?
    public SingleChunkPacket Build();
}