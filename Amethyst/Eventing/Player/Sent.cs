using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Player;

public sealed record Sent(string Message) : Event<IPlayer>;