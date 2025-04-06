namespace Amethyst.Abstractions.Entities;

public readonly struct Position(double x, double y, double z)
{
    public double X => x;

    public double Y => y;

    public double Z => z;

    public static Position operator -(Position left, Position right)
    {
        return new Position(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
}