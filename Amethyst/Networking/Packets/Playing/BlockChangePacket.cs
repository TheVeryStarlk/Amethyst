using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class BlockChangePacket : IOutgoingPacket
{
    public int Identifier => 0x23;

    public required Position Position { get; init; }

    public required Block Block { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WritePosition(Position);
        writer.WriteVariableInteger(Block.Type << 4 | (Block.Metadata & 15));
    }
}