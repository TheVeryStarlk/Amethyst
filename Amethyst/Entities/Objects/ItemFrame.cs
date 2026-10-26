using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class ItemFrame(IWorld world) : Hanging(world), IItemFrame
{
    public Item Item { get; set; }

    public byte Rotation { get; set; }
}