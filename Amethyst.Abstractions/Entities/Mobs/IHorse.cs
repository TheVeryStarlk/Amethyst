namespace Amethyst.Abstractions.Entities.Mobs;

public interface IHorse : IAgeable
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

public enum HorseType
{
    Horse,
    Donkey,
    Mule,
    Zombie,
    Skeleton
}

public enum HorseColor
{
    White,
    Creamy,
    Chestnut,
    Brown,
    Black,
    Gray,
    DarkBrown
}

public enum HorseMarkings
{
    None,
    White,
    WhiteField,
    WhiteDots,
    BlackDots
}

public enum HorseArmor
{
    None,
    Iron,
    Gold,
    Diamond
}