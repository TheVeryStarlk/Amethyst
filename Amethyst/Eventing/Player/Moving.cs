using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Moving(Location location) : Event<IPlayer>
{
    public Location Location => location;
}