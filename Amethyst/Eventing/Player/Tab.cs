using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Tab(string behind) : Event<IPlayer>
{
    public string Behind => behind;

    public string[] Matches { get; set; } = [];
}