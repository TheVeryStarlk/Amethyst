using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class Snowball(IWorld world) : Spawnable(world), ISnowball;