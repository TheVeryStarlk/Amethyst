using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name, IGenerator generator) : IWorld
{
    public string Name => name;

    private readonly Dictionary<long, Region> regions = [];

    public Block GetBlock(Position position)
    {
        return GetRegion(position.ToRegion()).GetBlock(position.ToChunk());
    }

    public void SetBlock(Block block, Position position)
    {
        GetRegion(position.ToRegion()).SetBlock(block, position.ToChunk());
    }

    public Chunk GetChunk(Position position)
    {
        return GetRegion(position.ToRegion()).GetChunk(position);
    }

    private Region GetRegion(Position position)
    {
        var value = NumericsHelper.Encode(position.X, position.Z);

        if (regions.TryGetValue(value, out var region))
        {
            return region;
        }

        region = new Region(generator);

        regions[value] = region;

        return region;
    }
}