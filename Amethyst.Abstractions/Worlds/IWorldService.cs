namespace Amethyst.Abstractions.Worlds;

public interface IWorldService
{
    public IReadOnlyDictionary<string, IWorld> Worlds { get; }

    public void Create(string name, IGenerator generator);
}