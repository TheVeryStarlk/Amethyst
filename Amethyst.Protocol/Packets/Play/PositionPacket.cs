namespace Amethyst.Protocol.Packets.Play;

public abstract record PositionPacketBase(double X, double Y, double Z, bool OnGround) : IIngoingPacket
{
    public static int Identifier => 4;
}