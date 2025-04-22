using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Abstractions.Worlds;

public interface IChunk
{
    public Block this[int x, int y, int z] { get; set; }

    public Biome this[int x, int z] { get; set; }

    public SingleChunkPacket Build();
}