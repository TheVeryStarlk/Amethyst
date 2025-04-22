using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class MessagePacket(Message message, MessagePosition position) : IOutgoingPacket
{
    public int Identifier => 2;

    public Message Message => message;

    public MessagePosition Position => position;
}