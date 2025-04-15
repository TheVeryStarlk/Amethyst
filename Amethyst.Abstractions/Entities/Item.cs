namespace Amethyst.Abstractions.Entities;

public readonly struct Item(short type, byte amount, short durability)
{
    public short Type => type;

    public byte Amount => amount;

    public short Durability => durability;
}