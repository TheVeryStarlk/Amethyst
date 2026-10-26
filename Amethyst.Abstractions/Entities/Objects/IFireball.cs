namespace Amethyst.Abstractions.Entities.Objects;

/// <remarks>Clients only read the shooter and accelerate it by its <see cref="ISpawnable.Velocity"/> when the entity is spawned.</remarks>
public interface IFireball : ISpawnable
{
    public IEntity? Shooter { get; set; }
}