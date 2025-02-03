namespace Amethyst.Protocol.Packets.Play;

internal sealed record PlayerPositionAndLookPacket(
    double X,
    double Y,
    double Z,
    float Yaw,
    float Pitch,
    bool OnGround) : IOutgoingPacket
{
    public int Identifier => 8;

    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    public void Write(ref SpanWriter writer)
    {
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteBoolean(OnGround);
    }
}