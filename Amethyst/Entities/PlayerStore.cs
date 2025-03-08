using Amethyst.Abstractions.Entities;

namespace Amethyst.Entities;

internal sealed class PlayerStore
{
    public IReadOnlyDictionary<string, IPlayer> Players => players;

    private readonly Dictionary<string, IPlayer> players = [];

    public void Add(IPlayer player)
    {
        players[player.Username] = player;
    }

    public void Remove(IPlayer player)
    {
        players.Remove(player.Username);
    }
}