using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record PositionLookPacket(Location Location, float Yaw, float Pitch) : IOutgoingPacket
{
    public int Length => sizeof(double) + sizeof(double) + sizeof(double) + sizeof(float) + sizeof(float) + sizeof(bool);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(Location.X)
            .WriteDouble(Location.Y)
            .WriteDouble(Location.Z)
            .WriteFloat(Yaw)
            .WriteFloat(Pitch);
    }
}