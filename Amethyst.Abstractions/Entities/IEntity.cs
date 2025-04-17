namespace Amethyst.Abstractions.Entities;

public interface IEntity
{
    public int Unique { get; }

    public Position Position { get; }

    public float Yaw { get; }

    public float Pitch { get; }

    public bool Ground { get; }
}