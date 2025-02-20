using System.Collections;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldStore : IEnumerable<KeyValuePair<IWorld, Dictionary<string, IPlayer>>>
{
    public Dictionary<string, IPlayer> this[IWorld world] => worlds[world];

    private readonly Dictionary<IWorld, Dictionary<string, IPlayer>> worlds = [];

    public void Add(IPlayer player)
    {
        if (worlds.TryGetValue(player.World, out var players))
        {
            players[player.Username] = player;
            return;
        }

        players = [];
        players[player.Username] = player;

        worlds.Add(player.World, players);
    }

    public void Remove(IPlayer player)
    {
        worlds[player.World].Remove(player.Username);
    }

    public IEnumerator<KeyValuePair<IWorld, Dictionary<string, IPlayer>>> GetEnumerator()
    {
        return worlds.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}