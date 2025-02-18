using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name, IGenerator generator) : IWorld
{
    public string Name { get; } = name;

    public IEnumerable<IRegion> Regions => regions;

    private readonly List<Region> regions = [];

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
        foreach (var existing in regions)
        {
            if (existing.Position == (position.X, position.Z))
            {
                return existing;
            }
        }

        var region = new Region(position.X, position.Z, generator);

        regions.Add(region);

        return region;
    }
}