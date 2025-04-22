using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class ListHeaderFooterPacket(Message header, Message footer) : IOutgoingPacket
{
    public int Identifier => 71;

    public Message Header => header;

    public Message Footer => footer;
}