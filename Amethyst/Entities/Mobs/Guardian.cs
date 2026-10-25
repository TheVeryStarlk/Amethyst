using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Guardian(IWorld world) : Mob(world, 30), IGuardian
{
    public bool IsElder { get; set; }

    public bool IsRetractingSpikes { get; set; }

    public IEntity? Target { get; set; }
}