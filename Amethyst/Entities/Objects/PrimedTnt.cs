using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class PrimedTnt(IWorld world) : Spawnable(world), IPrimedTnt;