using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class PositionLookSerializer(PositionLookPacket packet) : Serializer(packet)
{
    public override int Identifier => 8;

    public override int Length => sizeof(double) + sizeof(double) + sizeof(double) + sizeof(float) + sizeof(float) + sizeof(bool);

    public override void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(packet.Location.X)
            .WriteDouble(packet.Location.Y)
            .WriteDouble(packet.Location.Z)
            .WriteFloat(packet.Yaw)
            .WriteFloat(packet.Pitch);
    }
}