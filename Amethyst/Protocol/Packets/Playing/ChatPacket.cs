using Amethyst.Api;
using Amethyst.Api.Entities;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class ChatPacket : IIngoingPacket<ChatPacket>
{
    public static int Identifier => 0x01;

    public required string Message { get; init; }

    public static ChatPacket Read(MemoryReader reader)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(IServer server, IPlayer player, IClient client)
    {
        foreach (var other in server.Players.Where(other => other.Identifier != player.Identifier))
        {
            await other.KickAsync();
        }
    }
}