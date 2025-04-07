using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Moved(Position position, float yaw, float pitch) : IEvent<IPlayer>
{
    public Position Position => position;

    public float Yaw => yaw;

    public float Pitch => pitch;
}