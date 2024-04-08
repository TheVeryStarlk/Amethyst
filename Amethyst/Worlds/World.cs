using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Worlds;
using Amethyst.Entities;
using Amethyst.Protocol.Packets.Playing;

namespace Amethyst.Worlds;

internal sealed class World : IWorld
{
    public required IServer Server { get; init; }

    public required WorldType Type { get; set; }

    public required Difficulty Difficulty { get; set; }

    public required Dimension Dimension { get; set; }

    private readonly List<Region> regions = [];

    public Task TickAsync()
    {
        var players = Server.Players
            .Where(player => player.World == this)
            .Cast<Player>()
            .ToArray();

        foreach (var player in players)
        {
            var position = player.Vector.ToPosition().ToChunkCoordinates();
            var needed = new List<Chunk>();

            for (var x = position.X - player.ViewDistance; x < position.X + player.ViewDistance; x++)
            {
                for (var z = position.Z - player.ViewDistance; z < position.Z + player.ViewDistance; z++)
                {
                    var chunk = new Position(x, 0, z);
                    needed.Add(GetRegion(chunk).GetChunk(chunk));
                }
            }

            var unload = player.Chunks.Except(needed).ToArray();
            foreach (var unneeded in unload)
            {
                player.Client.Queue(
                    new ChunkPacket
                    {
                        Chunk = unneeded,
                        Unload = true
                    });

                player.Chunks.Remove(unneeded);
            }

            var chunks = new List<IChunk>();

            foreach (var need in needed.Where(need => !player.Chunks.Contains(need)))
            {
                chunks.Add(need);
                player.Chunks.Add(need);
            }

            if (chunks.Count != 0)
            {
                player.Client.Queue(
                    new ChunkBulkPacket
                    {
                        Chunks = chunks.Cast<Chunk>()
                    });
            }
        }

        return Task.CompletedTask;
    }

    public Block GetBlock(Position position)
    {
        throw new NotImplementedException();
    }

    public void SetBlock(Block block, Position position)
    {
    }

    private Region GetRegion(Position position)
    {
        position = position.ToChunkCoordinates();

        position = new Position(
            (long) Math.Floor((double) position.X / 32),
            0,
            (long) Math.Floor((double) position.Z / 32));

        var region = regions.FirstOrDefault(region => region.Position == position);

        if (region is not null)
        {
            return region;
        }

        region = new Region
        {
            Position = position
        };

        regions.Add(region);
        return region;
    }
}