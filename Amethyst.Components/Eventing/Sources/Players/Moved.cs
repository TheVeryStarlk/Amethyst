using Amethyst.Components.Entities;

namespace Amethyst.Components.Eventing.Sources.Players;

public sealed class Moved(Location location, float yaw, float pitch, bool onGround) : Event<IPlayer>
{
    public Location Location => location;

    public float Yaw => yaw;

    public float Pitch => pitch;

    public bool OnGround => onGround;
}