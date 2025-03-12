using Amethyst.Abstractions.Entities.Players;

namespace Amethyst.Abstractions.Worlds;

public interface IWorld
{
    public string Name { get; }

    public IEnumerable<IPlayer> Players { get; }

    public WorldType Type { get; }

    public Dimension Dimension { get; }

    public Difficulty Difficulty { get; }

    public IGenerator Generator { get; }

    public Block this[int x, int y, int z] { get; }

    public Biome this[int x, int z] { get; }
}