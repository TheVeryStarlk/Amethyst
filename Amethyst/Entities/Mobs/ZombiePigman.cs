using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class ZombiePigman(IWorld world) : Zombie(world), IZombiePigman;