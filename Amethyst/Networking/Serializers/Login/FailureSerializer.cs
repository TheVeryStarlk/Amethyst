using Amethyst.Abstractions.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed partial class FailureSerializer(string message) : ISerializer<FailurePacket, FailureSerializer>
{
    public static FailureSerializer Create(FailurePacket packet)
    {
        return new FailureSerializer(packet.Message.Serialize());
    }
}