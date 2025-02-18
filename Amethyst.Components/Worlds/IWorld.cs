namespace Amethyst.Components.Worlds;

public interface IWorld
{
    public string Name { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);
}