using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Entities;

// Think of a possibly better name.
internal sealed class PlayerRepository
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