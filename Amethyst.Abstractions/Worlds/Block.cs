namespace Amethyst.Abstractions.Worlds;

public readonly struct Block(int type, int metadata = 0)
{
    public int Type => type;

    public int Metadata => metadata;
}

public enum Digging
{
    Started,
    Cancelled,
    Finished,
    DropItemStack,
    DropItem,
    ShootArrowOrFinishEating
}

public enum BlockFace
{
    NegativeY,
    PositiveY,
    NegativeZ,
    PositiveZ,
    NegativeX,
    PositiveX,
    Self = byte.MaxValue
}