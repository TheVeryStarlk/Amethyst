using Amethyst.Components.Entities;

namespace Amethyst.Components.Eventing.Sources.Players;

public sealed class Sent(string message) : Event<IPlayer>
{
    public string Message => message;
}