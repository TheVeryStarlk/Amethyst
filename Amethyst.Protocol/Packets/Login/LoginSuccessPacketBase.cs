namespace Amethyst.Protocol.Packets.Login;

public abstract record LoginSuccessPacketBase(string Guid, string Username) : IOutgoingPacket
{
    public int Identifier => 2;
}