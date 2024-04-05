using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Worlds;

namespace Amethyst.Entities;

internal sealed class Player(IClient client) : IPlayer
{
    public required IServer Server { get; init; }

    public required IWorld World { get; set; }

    public int Identifier { get; } = Random.Shared.Next();

    public required Guid Guid { get; init; }

    public required string Username { get; init; }

    public VectorF Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public GameMode GameMode { get; set; }

    public Task SpawnAsync()
    {
        throw new NotImplementedException();
    }

    public Task SendChatAsync(Chat chat)
    {
        throw new NotImplementedException();
    }

    public Task KickAsync()
    {
        client.Stop();
        return Task.CompletedTask;
    }
}