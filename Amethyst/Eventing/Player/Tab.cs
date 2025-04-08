using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class CompletionRequest(string behind) : IEvent<IPlayer>
{
    public string Behind => behind;

    public string[] Matches { get; set; } = [];
}