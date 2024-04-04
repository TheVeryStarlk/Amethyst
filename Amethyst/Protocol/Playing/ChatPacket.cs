using Amethyst.Entities;

namespace Amethyst.Protocol.Playing;

internal sealed class ChatMessagePacket : IIngoingPacket<ChatMessagePacket>, IHandler
{
    public static int Identifier => 0x01;

    public required string Message { get; init; }

    public static ChatMessagePacket Read(MemoryReader reader)
    {
        throw new NotImplementedException();
    }

    public void Handle(Server server, Player player, Client client)
    {
        foreach (var other in server.Players.Where(other => other.Identifier != player.Identifier))
        {
            other.Kick();
        }
    }
}