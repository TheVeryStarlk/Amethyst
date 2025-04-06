namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record TabResponsePacket(string[] Matches) : IOutgoingPacket
{
    public int Identifier => 58;
}