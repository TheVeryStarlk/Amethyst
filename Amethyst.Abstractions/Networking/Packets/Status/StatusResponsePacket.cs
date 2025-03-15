using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed record StatusResponsePacket(string Name, int Numerical, int Maximum, int Online, Message Description, string Favicon) : IOutgoingPacket
{
    public int Identifier => 0;
}