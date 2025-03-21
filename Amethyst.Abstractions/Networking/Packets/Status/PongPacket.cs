namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed class PongPacket(long magic) : IOutgoingPacket
{
    public int Identifier => 1;

    public long Magic => magic;
}