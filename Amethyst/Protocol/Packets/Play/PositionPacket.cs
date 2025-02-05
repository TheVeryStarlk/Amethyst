namespace Amethyst.Protocol.Packets.Play;

internal sealed record PositionPacket(double X, double Y, double Z, bool OnGround) : IIngoingPacket<PositionPacket>
{
    public static int Identifier => 4;

    public static PositionPacket Create(SpanReader reader)
    {
        return new PositionPacket(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadBoolean());
    }
}