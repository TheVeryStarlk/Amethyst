namespace Amethyst.Components.Worlds;

public readonly record struct Position(int X, int Y, int Z)
{
    public static Position operator %(Position position, int value)
    {
        return new Position(position.X % value, position.Y % value, position.Z % value);
    }
}