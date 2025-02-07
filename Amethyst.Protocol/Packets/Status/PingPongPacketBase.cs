namespace Amethyst.Protocol.Packets.Status;

public abstract record PingPongPacketBase(long Magic) : IIngoingPacket, IOutgoingPacket
{
    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => Identifier;
}