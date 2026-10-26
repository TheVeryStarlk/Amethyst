using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class SmallFireball(IWorld world) : Fireball(world), ISmallFireball;