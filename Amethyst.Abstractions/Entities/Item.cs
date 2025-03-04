namespace Amethyst.Abstractions.Entities;

public record struct Item(short Type)
{
    public byte Amount { get; set; }

    public short Durability { get; set; }

    public bool IsBlock => Type is > 0 and < 256;
}