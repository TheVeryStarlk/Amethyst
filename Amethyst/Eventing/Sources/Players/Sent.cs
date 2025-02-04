using Amethyst.Entities;

namespace Amethyst.Eventing.Sources.Players;

public sealed class Sent(string message) : Event<Player>
{
    public string Message => message;
}