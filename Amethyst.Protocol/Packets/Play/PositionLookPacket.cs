namespace Amethyst.Protocol.Packets.Play;

public abstract record PositionLookPacketBase(
    double X,
    double Y,
    double Z,
    float Yaw,
    float Pitch,
    bool OnGround) : IIngoingPacket, IOutgoingPacket
{
    public static int Identifier => 6;

    int IOutgoingPacket.Identifier => 8;
}