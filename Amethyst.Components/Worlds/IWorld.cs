namespace Amethyst.Components.Worlds;

public interface IWorld
{
    public string Name { get; }

    public IEnumerable<IRegion> Regions { get; }

    public Block GetBlock(int x, int y, int z);

    public void SetBlock(Block block, int x, int y, int z);
}