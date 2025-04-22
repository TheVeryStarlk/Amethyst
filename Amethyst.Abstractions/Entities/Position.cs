namespace Amethyst.Abstractions.Entities;

public struct Position(double x, double y, double z)
{
    public double X { get; set; } = x;

    public double Y { get; set; } = y;

    public double Z { get; set; } = z;

    public static Position operator -(Position left, Position right)
    {
        return new Position(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
}