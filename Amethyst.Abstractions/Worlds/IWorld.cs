using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Worlds;

public interface IWorld
{
    public string Name { get; }

    public IReadOnlyDictionary<string, IPlayer> Players { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);

    public IChunk GetChunk(int x, int z);
}