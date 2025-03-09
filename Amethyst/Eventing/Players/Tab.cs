using Amethyst.Abstractions.Entities;

namespace Amethyst.Eventing.Players;

public sealed class Tab(string behind) : Event<IPlayer>
{
    public string Behind => behind;

    public string[] Matches { get; set; } = [];
}