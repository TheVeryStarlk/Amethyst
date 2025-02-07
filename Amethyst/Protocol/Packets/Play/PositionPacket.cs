namespace Amethyst.Protocol.Packets.Play;

internal sealed record PositionPacket(double X, double Y, double Z, bool OnGround)
    : PositionPacketBase(X, Y, Z, OnGround), ICreatable<PositionPacketBase>
{
    public static PositionPacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PositionPacket(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadBoolean());
    }
}