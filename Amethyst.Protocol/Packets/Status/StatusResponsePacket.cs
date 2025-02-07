namespace Amethyst.Protocol.Packets.Status;

public abstract record StatusResponsePacketBase(string Message) : IOutgoingPacket
{
    public int Identifier => 0;
}