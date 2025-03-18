using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

internal sealed record Joined : Event<IPlayer>;