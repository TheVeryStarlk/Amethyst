namespace Amethyst.Protocol.Packets.Play;

public abstract record LookPacketBase(float Yaw, float Pitch, bool OnGround) : IIngoingPacket
{
    public static int Identifier => 5;
}