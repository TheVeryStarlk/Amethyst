using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Entities;

internal sealed class PlayerRepository
{
    public IReadOnlyDictionary<string, IPlayer> Players => online;

    private readonly Dictionary<string, IPlayer> online = [];

    public void Add(IPlayer player)
    {
        online[player.Username] = player;
    }

    public void Remove(IPlayer player)
    {
        online.Remove(player.Username);
    }
}