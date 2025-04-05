using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Looking(float yaw, float pitch) : Event<IPlayer>
{
    public float Yaw => yaw;

    public float Pitch => pitch;
}