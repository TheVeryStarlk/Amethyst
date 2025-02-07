namespace Amethyst.Protocol.Packets.Play;

public abstract record KeepAlivePacketBase(int Magic) : IIngoingPacket, IOutgoingPacket
{
    public static int Identifier => 0;

    int IOutgoingPacket.Identifier => 0;
}