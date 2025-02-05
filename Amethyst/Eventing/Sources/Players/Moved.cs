using Amethyst.Entities;

namespace Amethyst.Eventing.Sources.Players;

public sealed class Moved(
    double x,
    double y,
    double z,
    float yaw,
    float pitch,
    bool onGround) : Event<Player>
{
    public double X => x;

    public double Y => y;

    public double Z => z;

    public float Yaw => yaw;

    public float Pitch => pitch;

    public bool OnGround => onGround;
}