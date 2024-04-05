using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Api.Worlds;

namespace Amethyst.Worlds;

internal sealed class World : IWorld
{
    public required IServer Server { get; init; }

    public IEnumerable<IPlayer> Players => players;

    public required WorldType Type { get; set; }

    public required Difficulty Difficulty { get; set; }

    public required Dimension Dimension { get; set; }

    private readonly List<IPlayer> players = [];

    public Task SpawnAsync(IEntity entity)
    {
        if (entity is IPlayer player)
        {
            players.Add(player);
        }

        return Task.CompletedTask;
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