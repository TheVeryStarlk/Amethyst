namespace Amethyst.Abstractions.Worlds;

public readonly record struct Position(int X, int Y, int Z)
{
    public static Position From(long value)
    {
        return new Position((int) (value >> 38), (int) (value >> 26) & 0xFFF, (int) (value << 38 >> 38));
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