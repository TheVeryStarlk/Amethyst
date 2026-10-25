using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities.Objects;

/// <remarks>Clients only read the block when the entity is spawned.</remarks>
public interface IFallingBlock : ISpawnable
{
    public Block Block { get; set; }
}