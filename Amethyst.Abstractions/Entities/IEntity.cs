namespace Playground.Abstractions.Entities;

public interface IEntity
{
    public int Identifier { get; }

    public Location Location { get; }

    public float Pitch { get; }

    public float Yaw { get; }

    public bool OnGround { get; }
}