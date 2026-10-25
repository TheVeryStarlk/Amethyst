using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Mooshroom(IWorld world) : Cow(world), IMooshroom;