using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Wither(IWorld world) : Mob(world, 300), IWither
{
    public IEntity? CenterTarget { get; set; }

    public IEntity? RightTarget { get; set; }

    public IEntity? LeftTarget { get; set; }

    public int InvulnerableTime { get; set; }
}