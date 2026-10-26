using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class MagmaCube(IWorld world) : Slime(world), IMagmaCube;