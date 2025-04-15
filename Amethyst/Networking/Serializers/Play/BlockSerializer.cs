using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class BlockSerializer(Position position, int block) : ISerializer<BlockPacket, BlockSerializer>
{
    public int Length => sizeof(long) + Variable.GetByteCount(block);

    public static BlockSerializer Create(BlockPacket packet)
    {
        return new BlockSerializer(packet.Position, packet.Block.Type << 4 | packet.Block.Metadata & 15);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WritePosition(position)
            .WriteVariableInteger(block);
    }
}