using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Protocol.Packets.Playing;

namespace Amethyst.Entities;

internal sealed class Player(IClient client) : EntityBase, IPlayer
{
    public required Guid Guid { get; init; }

    public required string Username { get; init; }

    public GameMode GameMode { get; set; }

    public Task SendChatAsync(Chat chat, ChatPosition position)
    {
        client.Queue(
            new ChatPacket
            {
                Chat = chat,
                Position = position
            });

        return Task.CompletedTask;
    }

    public Task KickAsync()
    {
        client.Stop();
        return Task.CompletedTask;
    }
}