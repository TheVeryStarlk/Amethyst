using Amethyst.Components.Entities;

namespace Amethyst.Entities;

internal sealed class PlayerStore : IPlayerStore
{
    public IReadOnlyDictionary<string, IPlayer> Players => players;

    private readonly Dictionary<string, IPlayer> players = [];

    public bool TryAdd(IPlayer player)
    {
        return players.TryAdd(player.Username, player);
    }

    public void Remove(IPlayer player)
    {
        players.Remove(player.Username);
    }
}