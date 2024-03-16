using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Api.Levels;

public interface IChunk
{
    public (long X, long Z) Position { get; }

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);

    public (byte[] Payload, ushort Bitmask) Serialize();
}