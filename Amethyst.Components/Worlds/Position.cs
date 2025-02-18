namespace Amethyst.Components.Worlds;

public readonly record struct Position(int X, int Y, int Z)
{
    public Position ToRegion()
    {
        return new Position(X >> 5, Y >> 5, Z >> 5);
    }

    public Position ToChunk()
    {
        return new Position(X >> 4, Y >> 4, Z >> 4);
    }

    public Position ToSection()
    {
        return new Position(X % 16, Y % 16, Z % 16);
    }
}