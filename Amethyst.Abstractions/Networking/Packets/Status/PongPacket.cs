namespace Amethyst.Abstractions.Networking.Packets.Status;

internal sealed class PongPacket(long magic) : IOutgoingPacket
{
    public int Identifier => 1;

    public long Magic => magic;
}