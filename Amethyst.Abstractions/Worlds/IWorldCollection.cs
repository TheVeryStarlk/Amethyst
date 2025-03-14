namespace Amethyst.Abstractions.Worlds;

public interface IWorldCollection : IReadOnlyCollection<IWorld>
{
    public void Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator);
}