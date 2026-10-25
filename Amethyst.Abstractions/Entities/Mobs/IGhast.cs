namespace Amethyst.Abstractions.Entities.Mobs;

public interface IGhast : IMob
{
    public bool IsAttacking { get; set; }
}