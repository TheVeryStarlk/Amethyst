namespace Amethyst.Components.Worlds;

public interface IWorldStore
{
    public IWorld this[string name] { get; }

    public int Count { get; }

    public void Create(string name);
}