using System.Collections.Concurrent;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Streams chunks around players as they move, within their view distance.
/// </summary>
public sealed class ChunkSubscriber : ISubscriber
{
    private readonly ConcurrentDictionary<IPlayer, Tracker> trackers = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Moved>((source, _) => Stream(source));
            consumer.On<Left>((source, _) => trackers.TryRemove(source, out var tracker));
        });
    }

    private void Stream(IPlayer player)
    {
        var tracker = trackers.GetOrAdd(player, _ => new Tracker());
        var center = (X: ToChunk(player.Position.X), Z: ToChunk(player.Position.Z));

        if (tracker.World != player.World)
        {
            // The client clears its chunks when changing worlds.
            tracker.World = player.World;
            tracker.Loaded.Clear();
        }
        else if (tracker.Center == center && tracker.Distance == player.ViewDistance)
        {
            return;
        }

        tracker.Center = center;
        tracker.Distance = player.ViewDistance;

        var alive = new HashSet<(int X, int Z)>();

        for (var x = center.X - player.ViewDistance; x <= center.X + player.ViewDistance; x++)
        {
            for (var z = center.Z - player.ViewDistance; z <= center.Z + player.ViewDistance; z++)
            {
                alive.Add((x, z));
            }
        }

        foreach (var dead in tracker.Loaded.Where(chunk => !alive.Contains(chunk)).ToArray())
        {
            player.Client.Write(new SingleChunkPacket(dead.X, dead.Z, [], 0));
            tracker.Loaded.Remove(dead);
        }

        var closest = alive.OrderBy(chunk => (chunk.X - center.X) * (chunk.X - center.X) + (chunk.Z - center.Z) * (chunk.Z - center.Z));

        foreach (var chunk in closest)
        {
            if (tracker.Loaded.Add(chunk))
            {
                player.Client.Write(player.World[chunk.X, chunk.Z].Build());
            }
        }
    }

    private static int ToChunk(double value)
    {
        return (int) Math.Floor(value) >> 4;
    }

    // Only ever touched by the player's own client, so it does not need to be thread-safe.
    private sealed class Tracker
    {
        public IWorld? World { get; set; }

        public (int X, int Z) Center { get; set; }

        public byte Distance { get; set; }

        public HashSet<(int X, int Z)> Loaded { get; } = [];
    }
}
