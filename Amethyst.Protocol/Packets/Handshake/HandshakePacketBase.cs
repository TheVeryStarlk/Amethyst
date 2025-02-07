namespace Amethyst.Protocol.Packets.Handshake;

public abstract record HandshakePacketBase(
    int Version,
    string Address,
    ushort Port,
    int State) : IIngoingPacket
{
    public static int Identifier => 0;
}