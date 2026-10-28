using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities.Objects;

public interface IFallingBlock : ISpawnable
{
    public Block Block { get; set; }
}