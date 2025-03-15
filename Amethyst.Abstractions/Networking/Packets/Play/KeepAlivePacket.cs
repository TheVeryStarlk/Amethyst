namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record KeepAlivePacket(long Magic) : IOutgoingPacket
{
    public int Identifier => 0;
}