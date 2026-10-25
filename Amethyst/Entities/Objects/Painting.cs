using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class Painting(IWorld world) : Hanging(world), IPainting
{
    public Motive Motive { get; set; }
}