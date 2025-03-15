using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record DisconnectPacket(Message Message) : IOutgoingPacket
{
    public int Identifier => 64;
}