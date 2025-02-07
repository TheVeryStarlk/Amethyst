namespace Amethyst.Protocol.Packets.Login;

public abstract record LoginStartPacketBase(string Username) : IIngoingPacket
{
    public static int Identifier => 0;
}