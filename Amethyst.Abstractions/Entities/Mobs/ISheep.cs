namespace Amethyst.Abstractions.Entities.Mobs;

public interface ISheep : IAgeable
{
    public DyeColor Color { get; set; }

    public bool IsSheared { get; set; }
}