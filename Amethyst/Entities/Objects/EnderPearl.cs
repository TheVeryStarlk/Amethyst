using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class EnderPearl(IWorld world) : Spawnable(world), IEnderPearl;