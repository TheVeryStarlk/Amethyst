using Amethyst.Components.Entities;

namespace Amethyst.Components.Worlds;

public interface IWorld
{
    public string Name { get; }

    public IReadOnlyDictionary<string, IPlayer> Players { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);
}