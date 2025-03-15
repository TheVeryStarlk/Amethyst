using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Packets.Status;

internal sealed record StatusRequestPacket : IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0;

    public static StatusRequestPacket Create(ReadOnlySpan<byte> span)
    {
        return new StatusRequestPacket();
    }

    public void Process(Client client)
    {
        client.Write(new StatusResponsePacket("a", 1, 2, 3, Message.Simple("a"), ""));
    }
}