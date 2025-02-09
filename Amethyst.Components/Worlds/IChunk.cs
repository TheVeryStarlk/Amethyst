namespace Amethyst.Components.Worlds;

public interface IChunk
{
    public int X { get; }

    public int Z { get; }

    public Block GetBlock(long x, long y, long z);

    public void SetBlock(Block block, long x, long y, long z);
}