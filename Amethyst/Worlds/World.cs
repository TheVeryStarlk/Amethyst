using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed record World(string Name, WorldType Type, Dimension Dimension, Difficulty Difficulty, IGenerator Generator) : IWorld
{
    public Difficulty Difficulty { get; set; } = Difficulty;

    // To be implemented...
    public IEnumerable<IPlayer> Players => [];

    private readonly Dictionary<long, Region> regions = [];

    public Block this[int x, int y, int z]
    {
        get => GetRegion(x.ToChunk(), z.ToChunk())[x, y, z];
        set => GetRegion(x.ToChunk(), z.ToChunk())[x, y, z] = value;
    }

    public IChunk this[int x, int z] => GetRegion(x, z)[x, z];

    private Region GetRegion(int x, int z)
    {
        var value = NumericUtility.Encode(x >> 5, z >> 5);

        if (regions.TryGetValue(value, out var region))
        {
            return region;
        }

        region = new Region(this, Generator);
        regions[value] = region;

        return region;
    }
}