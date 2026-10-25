using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class CaveSpider(IWorld world) : Spider(world, 12), ICaveSpider;