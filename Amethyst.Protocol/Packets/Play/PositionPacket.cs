namespace Amethyst.Protocol.Packets.Play;

public sealed record PositionPacket(double X, double Y, double Z, bool OnGround) : IIngoingPacket<PositionPacket>
{
    public static int Identifier => 4;

    static PositionPacket IIngoingPacket<PositionPacket>.Create(SpanReader reader)
    {
        return new PositionPacket(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadBoolean());
    }
}