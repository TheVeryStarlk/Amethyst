using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class World
{
    public IEnumerable<IRegion> Regions => regions;

    private readonly List<Region> regions = [];

    public Block GetBlock(int x, int y, int z)
    {
        return GetRegion(x, y, z).GetBlock(x, y, z);
    }

    public void SetBlock(Block block, int x, int y, int z)
    {
        GetRegion(x, y, z).SetBlock(block, x, y, z);
    }

    private Region GetRegion(int x, int y, int z)
    {
        x = (int) Math.Floor((double) (x >> 4) / 32);
        z = (int) Math.Floor((double) (z >> 4) / 32);

        var region = regions.FirstOrDefault(region => region.X == x && region.Z == z);

        if (region is not null)
        {
            return region;
        }

        region = new Region(x, z);
        regions.Add(region);

        return region;
    }
}