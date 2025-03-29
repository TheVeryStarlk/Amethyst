namespace Amethyst.Abstractions.Worlds;

// Probably should have a collection of hard-coded block types.
public readonly struct Block(int type, int metadata = 0)
{
    public int Type => type;

    public int Metadata => metadata;
}

public static class Blocks
{
    public static Block Air { get; } = new(0, 1);
}