using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class DisconnectPacket(Message message) : IOutgoingPacket
{
    public int Identifier => 64;

    public Message Message => message;
}