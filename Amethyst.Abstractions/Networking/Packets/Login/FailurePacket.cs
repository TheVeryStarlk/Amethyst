using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Login;

internal sealed class FailurePacket(Message message) : IOutgoingPacket
{
    public int Identifier => 0;

    public Message Message => message;
}