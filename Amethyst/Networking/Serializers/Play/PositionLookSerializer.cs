using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class PositionLookSerializer(double x, double y, double z, float yaw, float pitch, byte flags) : ISerializer<PositionLookPacket, PositionLookSerializer>
{
    public static PositionLookSerializer Create(PositionLookPacket packet)
    {
        // No flags are set, so every value is absolute.
        return new PositionLookSerializer(packet.Position.X, packet.Position.Y, packet.Position.Z, packet.Yaw, packet.Pitch, 0);
    }
}