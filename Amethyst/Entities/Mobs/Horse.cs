using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Mobs;

internal sealed class Horse(IWorld world) : Ageable(world, 20), IHorse
{
    public HorseType Type { get; set; }

    public HorseColor Color { get; set; }

    public HorseMarkings Markings { get; set; }

    public HorseArmor Armor { get; set; }

    public bool IsTamed { get; set; }

    public bool HasSaddle { get; set; }

    public bool HasChest { get; set; }

    public bool IsBred { get; set; }

    public bool IsGrazing { get; set; }

    public bool IsRearing { get; set; }

    public bool IsMouthOpen { get; set; }

    public Guid? Owner { get; set; }
}