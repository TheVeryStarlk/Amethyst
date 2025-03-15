namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed record PongPacket(long Magic) : IOutgoingPacket
{
    public int Identifier => 1;
}