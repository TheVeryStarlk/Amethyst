using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class BlockSerializer(Position position, [Variable] int block) : ISerializer<BlockPacket, BlockSerializer>
{
    public static BlockSerializer Create(BlockPacket packet)
    {
        return new BlockSerializer(packet.Position, packet.Block.Type << 4 | packet.Block.Metadata & 15);
    }
}