using Amethyst.Abstractions.Entities;

namespace Amethyst.Entities;

internal abstract class Entity : IEntity
{
    public int Unique { get; } = Track.Instance.Next();

    public Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool Ground { get; set; }
}

internal sealed class Track
{
    public static Track Instance { get; } = new();

    private int track;

    public int Next()
    {
        return Interlocked.Increment(ref track);
    }
}