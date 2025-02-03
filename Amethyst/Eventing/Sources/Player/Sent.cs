using Amethyst.Abstractions.Entities;

namespace Amethyst.Eventing.Sources.Player;

public sealed class Sent(string message) : Event<IPlayer>
{
    public string Message => message;
}