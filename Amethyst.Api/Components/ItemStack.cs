namespace Amethyst.Api.Components;

public sealed class ItemStack
{
    public required short Type { get; init; }

    public byte Amount { get; set; }

    public short Durability { get; set; }

    public bool IsBlock => Type is > 0 and < 256;
}