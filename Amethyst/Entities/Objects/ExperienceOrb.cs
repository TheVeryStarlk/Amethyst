using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class ExperienceOrb(IWorld world) : Spawnable(world), IExperienceOrb
{
    public short Count { get; set; } = 1;
}