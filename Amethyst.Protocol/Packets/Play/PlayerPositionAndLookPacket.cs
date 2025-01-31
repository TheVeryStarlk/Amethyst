namespace Amethyst.Protocol.Packets.Play;

public sealed record PlayerPositionAndLookPacket(
    double X,
    double Y,
    double Z,
    float Yaw,
    float Pitch,
    bool OnGround) : IOutgoingPacket
{
    public int Identifier => 8;

    int IOutgoingPacket.Length => sizeof(double)
                                  + sizeof(double)
                                  + sizeof(double)
                                  + sizeof(float)
                                  + sizeof(float)
                                  + sizeof(bool);

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