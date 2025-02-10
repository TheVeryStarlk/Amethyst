using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name) : IWorld
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

    private Region GetRegion(int x, int z)
    {
        x = (int) Math.Floor((double) (x >> 4) / 32);
        z = (int) Math.Floor((double) (z >> 4) / 32);

        var region = regions.FirstOrDefault(region => region.X == x && region.Z == z);

        if (region is not null)
        {
            return region;
        }

        region = new Region
        {
            X = x,
            Z = z
        };

        regions.Add(region);

        return region;
    }
}