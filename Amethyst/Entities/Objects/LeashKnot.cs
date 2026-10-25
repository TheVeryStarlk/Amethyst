using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class LeashKnot(IWorld world) : Spawnable(world), ILeashKnot;