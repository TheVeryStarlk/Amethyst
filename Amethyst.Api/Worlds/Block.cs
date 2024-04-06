namespace Amethyst.Api.Worlds;

public readonly record struct Block
{
    public required int Type { get; init; }

    public int Metadata { get; init; }
}

public enum BlockFace
{
    NegativeY,
    PositiveY,
    NegativeZ,
    PositiveZ,
    NegativeX,
    PositiveX
}