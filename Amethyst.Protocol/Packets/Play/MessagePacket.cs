namespace Amethyst.Protocol.Packets.Play;

public abstract record MessagePacketBase(string Message, byte Position) : IIngoingPacket, IOutgoingPacket
{
    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => 2;
}