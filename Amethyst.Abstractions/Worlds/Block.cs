namespace Amethyst.Abstractions.Worlds;

public readonly record struct Block(int Type, int Metadata = 0);

public enum BlockFace
{
    NegativeY,
    PositiveY,
    NegativeZ,
    PositiveZ,
    NegativeX,
    PositiveX
}