using Amethyst.Abstractions.Entities;

namespace Amethyst.Entities;

internal abstract class Entity : IEntity
{
    public int Identifier { get; } = Track.Next();

    public Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool Ground { get; set; }
}

internal static class Track
{
    private static int track;

    public static int Next()
    {
        return Interlocked.Increment(ref track);
    }
}