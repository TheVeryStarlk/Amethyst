using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Moving(Position position, float yaw, float pitch) : Event<IPlayer>
{
    public Position Position => position;

    public float Yaw => yaw;

    public float Pitch => pitch;
}