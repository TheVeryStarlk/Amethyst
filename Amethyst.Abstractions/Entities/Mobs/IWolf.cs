namespace Amethyst.Abstractions.Entities.Mobs;

public interface IWolf : ITameable
{
    public bool IsAngry { get; set; }

    public bool IsBegging { get; set; }

    public DyeColor CollarColor { get; set; }
}