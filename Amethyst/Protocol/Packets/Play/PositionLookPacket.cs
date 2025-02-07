namespace Amethyst.Protocol.Packets.Play;

internal sealed record PositionLookPacket(double X, double Y, double Z, float Yaw, float Pitch, bool OnGround)
    : PositionLookPacketBase(X, Y, Z, Yaw, Pitch, OnGround), ICreatable<PositionLookPacketBase>, IWriteable
{
    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    public static PositionLookPacketBase Create(ReadOnlySpan<byte> span)
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