using Amethyst.Components.Entities;
using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name, IGenerator generator) : IWorld
{
    public string Name => name;

    public IReadOnlyDictionary<string, IPlayer> Players => players;

    private readonly Dictionary<string, IPlayer> players = [];
    private readonly Dictionary<long, Region> regions = [];

    public void AddPlayer(IPlayer player)
    {
        players.Add(player.Username, player);
    }

    public void RemovePlayer(IPlayer player)
    {
        players.Remove(player.Username);
    }

    public Block GetBlock(Position position)
    {
        return GetRegion(position.X, position.Z).GetBlock(position.ToChunk());
    }

    public void SetBlock(Block block, Position position)
    {
        GetRegion(position.X, position.Z).SetBlock(block, position.ToChunk());
    }

    public Chunk GetChunk(int x, int z)
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