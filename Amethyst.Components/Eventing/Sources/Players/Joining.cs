using Amethyst.Components.Entities;

namespace Amethyst.Components.Eventing.Sources.Players;

public sealed class Joining : Event<IPlayer>
{
    public byte GameMode { get; set; } = 1;
}