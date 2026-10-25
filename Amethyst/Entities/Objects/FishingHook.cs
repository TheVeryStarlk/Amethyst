using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class FishingHook(IWorld world) : Spawnable(world), IFishingHook
{
    public IPlayer? Owner { get; set; }
}