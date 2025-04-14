namespace Amethyst.Abstractions.Entities.Player;

public interface IPlayerRepository
{
    public IReadOnlyDictionary<string, IPlayer> Players { get; }
}