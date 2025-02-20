using System.Collections;
using Amethyst.Abstractions.Entities;

namespace Amethyst.Worlds;

internal sealed class PlayerStore : IEnumerable<IPlayer>
{
    private readonly Dictionary<string, IPlayer> players = [];

    public void Add(IPlayer player)
    {
        players[player.Username] = player;
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
        return GetEnumerator();
    }
}