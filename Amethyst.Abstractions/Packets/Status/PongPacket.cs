namespace Amethyst.Abstractions.Packets.Status;

internal sealed class PongPacket(long magic) : IOutgoingPacket
{
    public int Identifier => 1;

    public long Magic => magic;
}