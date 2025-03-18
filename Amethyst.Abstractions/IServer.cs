using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IReadOnlyDictionary<string, IWorld> Worlds { get; }

    public void Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator);

    public void Stop();
}