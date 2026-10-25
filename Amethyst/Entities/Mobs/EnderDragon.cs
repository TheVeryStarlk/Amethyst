using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class EnderDragon(IWorld world) : Mob(world, 200), IEnderDragon;