namespace Amethyst.Api.Components;

public readonly record struct Position(long X, long Y, long Z)
{
    public static Position operator %(Position position, int value)
    {
        return new Position(position.X % value, position.Y % value, position.Z % value);
    }

    public Position ToChunkCoordinates()
    {
        return new Position(X >> 4, 0, Z >> 4);
    }
}