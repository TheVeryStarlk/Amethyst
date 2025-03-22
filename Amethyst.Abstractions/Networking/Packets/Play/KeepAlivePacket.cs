namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class KeepAlivePacket(int magic) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Magic => magic;
}