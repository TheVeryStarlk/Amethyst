using Amethyst.Components.Entities;

namespace Amethyst.Components.Eventing.Sources.Players;

public sealed class Moved(double x, double y, double z, float yaw, float pitch, bool onGround) : Event<IPlayer>
{
    public double X => x;

    public double Y  => y;

    public double Z  => z;

    public float Yaw  => yaw;

    public float Pitch  => pitch;

    public bool OnGround  => onGround;
}