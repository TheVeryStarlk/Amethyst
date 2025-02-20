using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name, IGenerator generator, WorldStore worldStore) : IWorld
{
    public string Name => name;

    public IReadOnlyDictionary<string, IPlayer> Players => worldStore[this];

    private readonly Dictionary<long, Region> regions = [];

    public Block GetBlock(Position position)
    {
        return GetRegion(position.X, position.Z).GetBlock(position.ToChunk());
    }

    public void SetBlock(Block block, Position position)
    {
        GetRegion(position.X, position.Z).SetBlock(block, position.ToChunk());
    }

    public IChunk GetChunk(int x, int z)
    {
        return GetRegion(x, z).GetChunk(x, z);
    }

    private Region GetRegion(int x, int z)
    {
        var value = NumericHelper.Encode(x >> 5, z >> 5);

        if (regions.TryGetValue(value, out var region))
        {
            return region;
        }

        region = new Region(generator);

        regions[value] = region;

        return region;
    }
}