using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class PositionLookSerializer(Location location, float yaw, float pitch) : ISerializer<PositionLookPacket, PositionLookSerializer>
{
    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    public static PositionLookSerializer Create(PositionLookPacket packet)
    {
        return new PositionLookSerializer(packet.Location, packet.Yaw, packet.Pitch);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(location.X)
            .WriteDouble(location.Y)
            .WriteDouble(location.Z)
            .WriteFloat(yaw)
            .WriteFloat(pitch)
            .WriteBoolean(false);
    }
}