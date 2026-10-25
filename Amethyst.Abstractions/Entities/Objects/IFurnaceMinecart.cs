namespace Amethyst.Abstractions.Entities.Objects;

public interface IFurnaceMinecart : IMinecart
{
    public bool IsPowered { get; set; }
}