using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class KeepAliveSerializer([Variable] int magic) : ISerializer<KeepAlivePacket, KeepAliveSerializer>
{
    public static KeepAliveSerializer Create(KeepAlivePacket packet)
    {
        return new KeepAliveSerializer(packet.Magic);
    }
}