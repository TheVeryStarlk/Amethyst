using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Entities.Objects;

public interface IFishingHook : ISpawnable
{
    public IPlayer? Owner { get; set; }
}