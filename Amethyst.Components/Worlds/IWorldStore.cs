namespace Amethyst.Components.Worlds;

public interface IWorldStore : IEnumerable<IWorld>
{
    public IWorld this[string name] { get; }

    public int Count { get; }

    public void Create(string name);
}