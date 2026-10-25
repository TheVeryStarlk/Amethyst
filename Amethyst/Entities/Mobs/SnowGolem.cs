using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class SnowGolem(IWorld world) : Mob(world, 4), ISnowGolem;