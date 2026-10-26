namespace Amethyst.Abstractions.Entities;

public interface ILiving : ISpawnable
{
    /// <summary>
    /// Starts at the entity's maximum health, entities without health are shown dying.
    /// </summary>
    public float Health { get; set; }

    /// <summary>
    /// The color of the potion particles around the entity, none if 0.
    /// </summary>
    public int PotionColor { get; set; }

    /// <summary>
    /// Whether the potion particles are faint, like the ones from beacons.
    /// </summary>
    public bool IsPotionAmbient { get; set; }

    /// <summary>
    /// The number of arrows stuck in the entity.
    /// </summary>
    public byte Arrows { get; set; }

    public Item Held { get; set; }

    public Item Helmet { get; set; }

    public Item Chestplate { get; set; }

    public Item Leggings { get; set; }

    public Item Boots { get; set; }
}