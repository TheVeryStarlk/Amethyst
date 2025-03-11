namespace Amethyst.Abstractions.Worlds;

public interface IWorld
{
    public string Name { get; }

    public WorldType Type { get; }

    public Dimension Dimension { get; }

    public Difficulty Difficulty { get; }

    public IGenerator Generator { get; }
}