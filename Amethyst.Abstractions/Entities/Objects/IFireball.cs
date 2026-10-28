namespace Amethyst.Abstractions.Entities.Objects;

public interface IFireball : ISpawnable
{
    public IEntity? Shooter { get; set; }
}