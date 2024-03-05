using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Entities;

internal sealed class Player(MinecraftClient client, string username) : IPlayer
{
    public string Username => username;

    public int Identifier { get; } = Random.Shared.Next();

    public Guid Guid { get; } = Guid.NewGuid();

    public async Task DisconnectAsync(ChatMessage reason)
    {
        await client.StopAsync();
    }
}