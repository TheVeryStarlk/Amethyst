using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Tab(string behind) : IEvent<IPlayer>
{
    public string Behind => behind;

    public string[] Matches { get; set; } = [];
}