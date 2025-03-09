using Amethyst.Abstractions.Entities;

namespace Amethyst.Eventing.Players;

public sealed class Sent(string message) : Event<IPlayer>
{
    public string Message => message;
}