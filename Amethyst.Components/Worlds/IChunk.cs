namespace Amethyst.Components.Worlds;

public interface IChunk
{
    public int X { get; }

    public int Z { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);
}