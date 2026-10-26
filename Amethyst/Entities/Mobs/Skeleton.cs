using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Skeleton(IWorld world) : Mob(world, 20), ISkeleton
{
    public SkeletonType Type { get; set; }
}