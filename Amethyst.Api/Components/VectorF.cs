namespace Amethyst.Api.Components;

public readonly record struct VectorF(double X, double Y, double Z)
{
    public Position ToPosition()
    {
        return new Position((long) X, (long) Y, (long) Z);
    }
}