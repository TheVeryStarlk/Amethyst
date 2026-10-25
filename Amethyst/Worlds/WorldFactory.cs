using System.Collections.Concurrent;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldFactory : IWorldFactory
{
    public IReadOnlyCollection<IWorld> Worlds => worlds;

    public IReadOnlyDictionary<string, IPlayer> Players => players;

    private readonly ConcurrentBag<IWorld> worlds = [];

    // Claiming usernames here in one step keeps them unique across worlds, even when two players join at once.
    private readonly ConcurrentDictionary<string, IPlayer> players = new(StringComparer.OrdinalIgnoreCase);

    public IWorld Create(string name, IGenerator generator, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful)
    {
        var world = new World(name, type, dimension, difficulty, generator, this);
        worlds.Add(world);

        return world;
    }

    /// <returns>False if a player with the same username, ignoring case, is already in any world.</returns>
    public bool TryAdd(IPlayer player)
    {
        return players.TryAdd(player.Username, player);
    }

    public void Remove(IPlayer player)
    {
        // Only remove this exact player, not a newer one that claimed the username since.
        players.TryRemove(new KeyValuePair<string, IPlayer>(player.Username, player));
    }
}