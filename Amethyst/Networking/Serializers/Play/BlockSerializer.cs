using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class BlockSerializer(long position, int block) : ISerializer<BlockPacket, BlockSerializer>
{
    public int Length => sizeof(long) + Variable.GetByteCount(block);

    public static BlockSerializer Create(BlockPacket packet)
    {
        var position = ((long) packet.Position.X & 0x3FFFFFF) << 38 | ((long) packet.Position.Y & 0xFFF) << 26 | (long) packet.Position.Z & 0x3FFFFFF;
        return new BlockSerializer(position, packet.Block.Type << 4 | packet.Block.Metadata & 15);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteLong(position)
            .WriteVariableInteger(block);
    }
}