using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities.Mobs;

public interface IEnderman : IMob
{
    public Block Carried { get; set; }

    public bool IsScreaming { get; set; }
}