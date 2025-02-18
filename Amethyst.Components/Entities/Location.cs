using Amethyst.Components.Worlds;

namespace Amethyst.Components.Entities;

public readonly record struct Location(double X, double Y, double Z)
{
    public Position ToPosition()
    {
        return new Position((int) X, (int) Y, (int) Z);
    }
}