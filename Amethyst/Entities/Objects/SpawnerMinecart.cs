using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class SpawnerMinecart(IWorld world) : Minecart(world), ISpawnerMinecart;