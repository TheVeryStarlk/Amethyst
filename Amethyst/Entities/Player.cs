using Amethyst.Api.Entities;

namespace Amethyst.Entities;

internal sealed class Player(IClient client) : EntityBase, IPlayer
{
    public required Guid Guid { get; init; }

    public required string Username { get; init;}

    public GameMode GameMode { get; set; }

    public Task KickAsync()
    {
        client.Stop();
        return Task.CompletedTask;
    }
}