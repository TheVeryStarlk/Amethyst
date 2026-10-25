using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal abstract class Living(IWorld world, float health) : Spawnable(world), ILiving
{
    public float Health { get; set; } = health;

    public int PotionColor { get; set; }

    public bool IsPotionAmbient { get; set; }

    public byte Arrows { get; set; }

    public Item Held { get; set; }

    public Item Helmet { get; set; }

    public Item Chestplate { get; set; }

    public Item Leggings { get; set; }

    public Item Boots { get; set; }
}