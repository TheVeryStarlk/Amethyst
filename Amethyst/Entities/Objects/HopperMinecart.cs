using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class HopperMinecart(IWorld world) : Minecart(world), IHopperMinecart;