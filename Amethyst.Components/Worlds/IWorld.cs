namespace Amethyst.Components.Worlds;

public interface IWorld
{
    public string Name { get; }

    public IEnumerable<IRegion> Regions { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);
}