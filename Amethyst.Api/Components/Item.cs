namespace Amethyst.Api.Components;

public sealed class Item
{
    public required short Type { get; init; }

    public byte Amount { get; set; }

    public short Durability { get; set; }

    public bool IsBlock => Type < 256;
}