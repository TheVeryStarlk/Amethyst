namespace Amethyst.Abstractions.Entities.Objects;

public interface IDroppedItem : ISpawnable
{
    public Item Item { get; set; }
}