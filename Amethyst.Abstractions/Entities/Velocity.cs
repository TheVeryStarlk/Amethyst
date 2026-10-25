namespace Amethyst.Abstractions.Entities;

/// <summary>
/// A velocity in blocks per tick, clients cap each axis to 3.9 blocks per tick.
/// </summary>
public struct Velocity(double x, double y, double z)
{
    public double X { get; set; } = x;

    public double Y { get; set; } = y;

    public double Z { get; set; } = z;
}