using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Sent(string message) : IEvent<IPlayer>
{
    public string Message => message;
}