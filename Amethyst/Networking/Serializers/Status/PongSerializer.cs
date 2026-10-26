using Amethyst.Abstractions.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed partial class PongSerializer(long magic) : ISerializer<PongPacket, PongSerializer>
{
    public static PongSerializer Create(PongPacket packet)
    {
        return new PongSerializer(packet.Magic);
    }
}