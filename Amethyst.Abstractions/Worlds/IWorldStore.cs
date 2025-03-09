namespace Amethyst.Abstractions.Worlds;

public interface IWorldStore
{
    public IReadOnlyDictionary<string, IWorld> Worlds { get; }

    public void Create(string name, IGenerator generator);
}