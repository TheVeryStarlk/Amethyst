using Amethyst.Abstractions.Entities;

namespace Amethyst.Entities;

internal abstract class Entity : IEntity
{
    public int Unique { get; } = Tracker.Instance.Next();

    public Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool Ground { get; set; }
}

internal sealed class Tracker
{
    public static Tracker Instance { get; } = new();

    private int current;

    public int Next()
    {
        return Interlocked.Increment(ref current);
    }
}