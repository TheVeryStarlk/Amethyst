namespace Amethyst.Protocol.Packets.Play;

public abstract record OnGroundPacketBase(bool Value) : IIngoingPacket
{
    public static int Identifier => 3;
}