using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Eventing.Players;

public sealed class Sent(string message) : Event<IPlayer>
{
    public string Message => message;
}