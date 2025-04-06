using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Moving(Location location, float yaw, float pitch) : Event<IPlayer>
{
    public Location Location => location;

    public float Yaw => yaw;

    public float Pitch => pitch;
}