using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class ExperienceBottle(IWorld world) : Spawnable(world), IExperienceBottle;