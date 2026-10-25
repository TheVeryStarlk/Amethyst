namespace Amethyst.Abstractions.Entities.Mobs;

public interface IZombie : IMob
{
    public bool IsChild { get; set; }

    public bool IsVillager { get; set; }

    /// <summary>
    /// Whether the zombie villager is shaking while being cured.
    /// </summary>
    public bool IsConverting { get; set; }
}