using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Abstractions.Worlds;

public interface IChunk
{
    public int X { get; }

    public int Z { get; }

    // Should this be public API?
    public SingleChunkPacket Build();
}