using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class FurnaceMinecart(IWorld world) : Minecart(world), IFurnaceMinecart
{
    public bool IsPowered { get; set; }
}