using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorld Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator);

    public void Stop();
}