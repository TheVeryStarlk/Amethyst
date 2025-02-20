namespace Amethyst.Abstractions.Entities;

public interface IEntity
{
    public int Identifier { get; }

    public Location Location { get; }

    public Angle Yaw { get; }

    public Angle Pitch { get; }

    public bool OnGround { get; }
}