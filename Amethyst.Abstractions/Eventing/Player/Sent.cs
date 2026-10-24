using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Sent(string message) : IEvent<IPlayer>
{
    public string Message => message;
}