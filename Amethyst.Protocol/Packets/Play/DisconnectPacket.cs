namespace Amethyst.Protocol.Packets.Play;

public abstract record DisconnectPacketBase(string Reason) : IOutgoingPacket
{
    public int Identifier => 64;
}