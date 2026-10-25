using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Entities.Objects;

/// <remarks>Clients only show hooks with an owner they can see, which is only read when the entity is spawned.</remarks>
public interface IFishingHook : ISpawnable
{
    public IPlayer? Owner { get; set; }
}