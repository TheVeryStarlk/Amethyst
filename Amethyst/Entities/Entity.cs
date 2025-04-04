using Amethyst.Abstractions.Entities;

namespace Amethyst.Entities;

internal abstract class Entity : IEntity
{
    public int Identifier { get; } = Interlocked.Increment(ref identifier);

    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    private static int identifier;
}