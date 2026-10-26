using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class DisconnectSerializer(string message) : ISerializer<DisconnectPacket, DisconnectSerializer>
{
    public static DisconnectSerializer Create(DisconnectPacket packet)
    {
        return new DisconnectSerializer(packet.Message.Serialize());
    }
}