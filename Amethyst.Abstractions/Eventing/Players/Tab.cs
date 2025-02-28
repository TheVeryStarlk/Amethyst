using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Eventing.Players;

public sealed class Tab(string behind) : Event<IPlayer>
{
    public string Behind => behind;

    public string[] Matches { get; set; } = [];
}