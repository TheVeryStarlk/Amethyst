using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class PositionLookPacket(Location location, float yaw, float pitch) : IOutgoingPacket
{
    public int Identifier => 8;

    public int Length => sizeof(double) + sizeof(double) + sizeof(double) + sizeof(float) + sizeof(float) + sizeof(bool);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(location.X)
            .WriteDouble(location.Y)
            .WriteDouble(location.Z)
            .WriteFloat(yaw)
            .WriteFloat(pitch);
    }
}