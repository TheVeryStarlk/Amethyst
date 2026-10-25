namespace Amethyst.Abstractions.Entities.Mobs;

public interface IVillager : IAgeable
{
    public Profession Profession { get; set; }
}

public enum Profession
{
    Farmer,
    Librarian,
    Priest,
    Blacksmith,
    Butcher
}