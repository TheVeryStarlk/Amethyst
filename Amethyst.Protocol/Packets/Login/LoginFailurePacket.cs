namespace Amethyst.Protocol.Packets.Login;

public abstract record LoginFailurePacketBase(string Reason) : IOutgoingPacket
{
    public int Identifier => 0;
}