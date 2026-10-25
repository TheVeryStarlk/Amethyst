using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class LargeFireball(IWorld world) : Fireball(world), ILargeFireball;