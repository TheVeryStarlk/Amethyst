namespace Amethyst.Protocol.Packets.Status;

public abstract record StatusRequestPacketBase : IIngoingPacket
{
    public static int Identifier => 0;
}