using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal abstract class Hanging(IWorld world) : Spawnable(world), IHanging
{
    public Direction Direction { get; set; }
}