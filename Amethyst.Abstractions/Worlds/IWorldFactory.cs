namespace Amethyst.Abstractions.Worlds;

public interface IWorldFactory
{
    public IWorld Create(string name, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful);

    public IWorld Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator);
}