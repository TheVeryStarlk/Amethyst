namespace Amethyst.Abstractions.Entities.Mobs;

/// <remarks>The tail is raised by its <see cref="ILiving.Health"/>.</remarks>
public interface IWolf : ITameable
{
    public bool IsAngry { get; set; }

    public bool IsBegging { get; set; }

    public DyeColor CollarColor { get; set; }
}