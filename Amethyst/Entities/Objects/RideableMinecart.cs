using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class RideableMinecart(IWorld world) : Minecart(world), IRideableMinecart;