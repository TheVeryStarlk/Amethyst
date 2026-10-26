using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Ocelot(IWorld world) : Tameable(world, 10), IOcelot
{
    public OcelotType Type { get; set; }
}