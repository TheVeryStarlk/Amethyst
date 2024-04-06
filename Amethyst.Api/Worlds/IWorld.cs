using Amethyst.Api.Components;

namespace Amethyst.Api.Worlds;

public interface IWorld
{
    public IServer Server { get; }

    public WorldType Type { get; set; }

    public Difficulty Difficulty { get; set; }

    public Dimension Dimension { get; set; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);
}