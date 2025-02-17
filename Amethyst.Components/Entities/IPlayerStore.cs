namespace Amethyst.Components.Entities;

public interface IPlayerStore
{
    public IReadOnlyDictionary<string, IPlayer> Players { get; }
}