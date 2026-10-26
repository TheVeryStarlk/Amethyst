namespace Amethyst.Abstractions.Entities.Mobs;

public interface ISpider : IMob
{
    public bool IsClimbing { get; set; }
}