using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;

namespace Amethyst.Worlds;

internal sealed class World(string name, IGenerator generator, PlayerStore playerStore) : IWorld
{
    public string Name => name;

    // Could this be more efficient?
    public IReadOnlyDictionary<string, IPlayer> Players => playerStore.Players.Where(pair => pair.Value.World == this).ToDictionary();

    private readonly Dictionary<long, Region> regions = [];

    public Block GetBlock(Position position)
    {
        return GetRegion(position.X >> 4, position.Z >> 4).GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        GetRegion(position.X >> 4, position.Z >> 4).SetBlock(block, position);
    }

    public IChunk GetChunk(int x, int z)
    {
        // This probably should not shift.
        return GetRegion(x, z).GetChunk(x << 4, z << 4);
    }

    private Region GetRegion(int x, int z)
    {
        var value = NumericHelper.Encode(x >> 5, z >> 5);

        if (regions.TryGetValue(value, out var region))
        {
            return region;
        }

        region = new Region(this, generator);

        regions[value] = region;

        return region;
    }
}