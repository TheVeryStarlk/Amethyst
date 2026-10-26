namespace Amethyst.Abstractions.Entities.Mobs;

public interface IRabbit : IAgeable
{
    public RabbitType Type { get; set; }
}

public enum RabbitType
{
    Brown,
    White,
    Black,
    BlackAndWhite,
    Gold,
    SaltAndPepper,
    Killer = 99
}