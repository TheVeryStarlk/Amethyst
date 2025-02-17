using System.Collections;
using Amethyst.Components.Entities;

namespace Amethyst.Entities;

internal sealed class PlayerStore : IPlayerStore
{
    public IPlayer this[string username] => players[username];

    public int Count => players.Count;

    private readonly Dictionary<string, IPlayer> players = [];

    public bool TryAdd(IPlayer player)
    {
        return players.TryAdd(player.Username, player);
    }

    public void Remove(IPlayer player)
    {
        players.Remove(player.Username);
    }

    public IEnumerator<IPlayer> GetEnumerator()
    {
        return players.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return players.Values.GetEnumerator();
    }
}