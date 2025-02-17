using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name, IGenerator generator) : IWorld
{
    public string Name { get; } = name;

    public IEnumerable<IRegion> Regions => regions;

    private readonly List<Region> regions = [];

    public Block GetBlock(Position position)
    {
        return GetRegion(position.X, position.Z).GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        GetRegion(position.X, position.Z).SetBlock(block, position);
    }

    public Chunk GetChunk(Position position)
    {
        return GetRegion(position.X >> 5, position.Z >> 5).GetChunk(position.X, position.Z);
    }

    private Region GetRegion(int x, int z)
    {
        var region = regions.FirstOrDefault(region => (region.Position.X, region.Position.Z) == (x, z));

        if (region is not null)
        {
            return region;
        }

        region = new Region(new Position(x, 0, z), generator);

        regions.Add(region);

        return region;
    }
}