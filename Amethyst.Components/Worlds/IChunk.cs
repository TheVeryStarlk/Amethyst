namespace Amethyst.Components.Worlds;

public interface IChunk
{
    public int X { get; }

    public int Z { get; }

    public Block GetBlock(int x, int y, int z);

    public void SetBlock(Block block, int x, int y, int z);
}