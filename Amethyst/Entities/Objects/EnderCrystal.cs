using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class EnderCrystal(IWorld world) : Spawnable(world), IEnderCrystal;