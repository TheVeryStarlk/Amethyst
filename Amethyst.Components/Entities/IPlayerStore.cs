namespace Amethyst.Components.Entities;

public interface IPlayerStore : IEnumerable<IPlayer>
{
    public IPlayer this[string username] { get; }

    public int Count { get; }
}