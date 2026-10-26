using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class ChestMinecart(IWorld world) : Minecart(world), IChestMinecart;