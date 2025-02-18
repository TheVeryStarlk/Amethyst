namespace Amethyst.Components.Worlds;

public interface IChunk
{
    public (int X, int Z) Position { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);
}