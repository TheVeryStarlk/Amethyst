using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities;

public readonly struct Item(short type, byte amount, short durability)
{
    public short Type => type;

    public byte Amount => amount;

    /// <remarks>Could be referred as <see cref="Block.Metadata"/>.</remarks>
    public short Durability => durability;
}