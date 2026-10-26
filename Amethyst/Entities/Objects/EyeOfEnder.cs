using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class EyeOfEnder(IWorld world) : Spawnable(world), IEyeOfEnder;