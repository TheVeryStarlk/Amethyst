namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class KeepAlivePacket(long magic) : IOutgoingPacket
{
    public int Identifier => 0;

    public long Magic => magic;
}