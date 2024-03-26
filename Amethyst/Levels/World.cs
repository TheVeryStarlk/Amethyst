using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Levels;
using Amethyst.Api.Levels.Blocks;
using Amethyst.Api.Levels.Generators;
using Amethyst.Networking.Packets.Playing;

namespace Amethyst.Levels;

internal sealed class World(Server server, string name, IWorldGenerator generator) : IWorld
{
    public string Name => name;

    public IWorldGenerator Generator { get; set; } = generator;

    public WorldType Type { get; set; }

    public Difficulty Difficulty { get; set; }

    public Dimension Dimension { get; set; }

    public long Age => DateTimeOffset.Now.Ticks;

    public long Time { get; set; }

    public IEnumerable<IRegion> Regions => regions;

    private readonly List<Region> regions = [];
    private readonly List<IPlayer> players = [];

    public async Task TickAsync()
    {
        Time++;

        server.BroadcastPacket(
            new TimeUpdatePacket
            {
                World = this
            },
            this);

        foreach (var player in players)
        {
            var position = new Position((int) player.Position.X >> 4, 0, (int) player.Position.Z >> 4);

            for (var x = position.X - 1; x < position.X + 1; x++)
            {
                for (var z = position.Z - 1; z < position.Z + 1; z++)
                {
                    if (player.Chunks.Any(predicate => predicate.X == x && predicate.Z == z))
                    {
                        continue;
                    }

                    var chunk = GetChunk(new Position((int) x, 0, (int) z));
                    player.Chunks.Add(new Position(chunk.Position.X, 0, chunk.Position.Z));

                    server.QueuePacket(
                        player,
                        new ChunkPacket
                        {
                            Chunk = chunk
                        });
                }
            }

            var others = players
                .Where(predicate => predicate.Username != player.Username)
                .Select(predicate => (IEntity) predicate)
                .ToArray();

            await player.UpdateEntitiesAsync(others);
        }
    }

    public async Task AddPlayerAsync(IPlayer player)
    {
        foreach (var other in players)
        {
            await player.SpawnPlayerAsync(other);
            await other.SpawnPlayerAsync(player);
        }

        players.Add(player);
    }

    public async Task RemovePlayerAsync(IPlayer player)
    {
        server.BroadcastPacket(
            new PlayerListItemPacket
            {
                Action = new RemovePlayerAction(),
                Players =
                [
                    player
                ]
            });

        await DestroyEntitiesAsync(player);
        players.Remove(player);
    }

    public async Task DestroyEntitiesAsync(params IEntity[] entities)
    {
        foreach (var player in entities.OfType<IPlayer>())
        {
            players.Remove(player);
        }

        foreach (var player in players)
        {
            await player.DestroyEntitiesAsync(entities);
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

        region = new Region((long) x, (long) z, Generator);
        regions.Add(region);
        return region;
    }
}