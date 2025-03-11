namespace Playground.Abstractions.Entities;

public readonly record struct Location(double X, double Y, double Z)
{
    public static Location operator -(Location left, Location right)
    {
        return new Location(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
}