using Amethyst.Abstractions.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed partial class SuccessSerializer(string guid, string username) : ISerializer<SuccessPacket, SuccessSerializer>
{
    public static SuccessSerializer Create(SuccessPacket packet)
    {
        return new SuccessSerializer(packet.Player.Guid.ToString(), packet.Player.Username);
    }
}