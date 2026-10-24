using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Moved(Position position, float yaw, float pitch) : IEvent<IPlayer>
{
    public Position Position => position;

    public float Yaw => yaw;

    public float Pitch => pitch;
}