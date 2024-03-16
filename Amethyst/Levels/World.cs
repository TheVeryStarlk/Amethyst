using Amethyst.Api.Components;
using Amethyst.Api.Levels;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Levels;

internal sealed class World(string name) : IWorld
{
    public string Name => name;

    public WorldType Type { get; set; }

    public Difficulty Difficulty { get; set; }

    public Dimension Dimension { get; set; }

    private readonly List<Region> regions = [];

    public void Generate()
    {
        for (var x = 0; x < 16; x++)
        {
            for (var z = 0; z < 16; z++)
            {
                SetBlock(new Block(7, 0), new Position(x, 0, z));
            }
        }
    }

    public Block GetBlock(Position position)
    {
        return GetRegion(position).GetBlock(position);
    }

    public void SetBlock(Block block, Position position)
    {
        GetRegion(position).SetBlock(block, position);
    }

    public IChunk GetChunk(Position position)
    {
        return GetRegion(position).GetChunk(position);
    }

    private Region GetRegion(Position position)
    {
        var (x, z) = (Math.Floor((double) position.X / 32), Math.Floor((double) position.Z / 32));
        var region = regions.FirstOrDefault(region => region.Position == (x, z));

        if (region is not null)
        {
            return region;
        }

        region = new Region((long) x, (long) z);
        regions.Add(region);
        return region;
    }
}