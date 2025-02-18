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

    public int AsIndex()
    {
        return (Y & 0xF) << 8 | (Z & 0xF) << 4 | X & 0xF;
    }
}