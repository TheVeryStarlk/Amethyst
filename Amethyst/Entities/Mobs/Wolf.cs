using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Wolf(IWorld world) : Tameable(world, 8), IWolf
{
    public bool IsAngry { get; set; }

    public bool IsBegging { get; set; }

    public DyeColor CollarColor { get; set; } = DyeColor.Red;
}