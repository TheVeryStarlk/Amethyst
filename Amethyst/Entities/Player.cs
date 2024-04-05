using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Api.Worlds;

namespace Amethyst.Entities;

internal sealed class Player(IServer server, IWorld world, IClient client) : IPlayer
{
    public IServer Server => server;

    public IWorld World { get; set; } = world;

    public int Identifier { get; } = Random.Shared.Next();

    public Task SpawnAsync()
    {
        throw new NotImplementedException();
    }

    public Task KickAsync()
    {
        client.Stop();
        return Task.CompletedTask;
    }
}