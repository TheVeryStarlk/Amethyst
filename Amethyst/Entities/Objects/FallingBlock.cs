using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class FallingBlock(IWorld world) : Spawnable(world), IFallingBlock
{
    public Block Block { get; set; } = Blocks.Sand;
}