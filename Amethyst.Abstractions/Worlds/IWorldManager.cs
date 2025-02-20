namespace Amethyst.Abstractions.Worlds;

public interface IWorldManager : IEnumerable<IWorld>
{
    public IWorld this[string name] { get; }

    public int Count { get; }

    public void Create(string name, IGenerator generator);
}