using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Api.Levels;

public interface IWorld
{
    public WorldType Type { get; set; }

    public Difficulty Difficulty { get; set; }

    public Dimension Dimension { get; set; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);

    public IChunk GetChunk(Position position);
}