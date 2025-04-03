using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldFactory : IWorldFactory
{
    public IWorld Create(string name, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful)
    {
        return new World(name, type, dimension, difficulty, EmptyGenerator.Instance);
    }

    public IWorld Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator)
    {
        return new World(name, type, dimension, difficulty, generator);
    }
}