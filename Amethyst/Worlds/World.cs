using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Api.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(Server server) : IWorld
{
    public IServer Server => server;

    public IEnumerable<IPlayer> Players => players;

    private readonly List<IPlayer> players = [];

    public async Task SpawnAsync(IEntity entity)
    {
        if (entity is IPlayer player)
        {
            players.Add(player);
        }

        await entity.SpawnAsync();
    }

    public Task DestroyAsync(params IEntity[] entities)
    {
        foreach (var entity in entities)
        {
            if (entity is IPlayer player)
            {
                players.Remove(player);
            }
        }

        return Task.CompletedTask;
    }
}