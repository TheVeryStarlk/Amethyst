using Amethyst.Components.Worlds;

namespace Amethyst.Components.Entities;

public readonly record struct Location(double X, double Y, double Z)
{
    public static Location operator -(Location left, Location right)
    {
        return new Location(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public Location ToAbsolute()
    {
        return new Location(X * 32D, Y * 32D, Z * 32D);
    }

    public Position ToPosition()
    {
        return new Position((int) X, (int) Y, (int) Z);
    }
}