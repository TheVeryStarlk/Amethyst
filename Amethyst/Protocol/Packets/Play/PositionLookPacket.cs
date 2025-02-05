namespace Amethyst.Protocol.Packets.Play;

public sealed record PositionLookPacket(
    double X,
    double Y,
    double Z,
    float Yaw,
    float Pitch,
    bool OnGround) : IIngoingPacket<PositionLookPacket>, IOutgoingPacket
{
    public static int Identifier => 6;

    int IOutgoingPacket.Identifier => 8;

    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    static PositionLookPacket IIngoingPacket<PositionLookPacket>.Create(SpanReader reader)
    {
        return new PositionLookPacket(
            reader.ReadDouble(),
            reader.ReadDouble(),
            reader.ReadDouble(),
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadBoolean());
    }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteBoolean(OnGround);
    }
}