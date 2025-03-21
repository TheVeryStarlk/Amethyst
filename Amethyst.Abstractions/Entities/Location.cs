namespace Amethyst.Abstractions.Entities;

public readonly struct Location(double x, double y, double z)
{
    public double X => x;

    public double Y => y;

    public double Z => z;

    public static Location operator -(Location left, Location right)
    {
        return new Location(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
}