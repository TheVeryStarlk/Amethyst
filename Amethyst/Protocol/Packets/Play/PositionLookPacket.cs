using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record PositionLookPacket(double X, double Y, double Z, float Yaw, float Pitch, bool OnGround) : IIngoingPacket<PositionLookPacket>, IOutgoingPacket
{
    public static int Identifier => 6;

    int IOutgoingPacket.Identifier => 8;

    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    public static PositionLookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionLookPacket(
            reader.ReadDouble(),
            reader.ReadDouble(),
            reader.ReadDouble(),
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadBoolean());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(X)
            .WriteDouble(Y)
            .WriteDouble(Z)
            .WriteFloat(Yaw)
            .WriteFloat(Pitch)
            .WriteBoolean(OnGround);
    }
}