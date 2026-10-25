namespace Amethyst.Abstractions.Entities.Mobs;

public interface IBlaze : IMob
{
    /// <summary>
    /// Whether the blaze is burning up to shoot fireballs.
    /// </summary>
    public bool IsCharging { get; set; }
}