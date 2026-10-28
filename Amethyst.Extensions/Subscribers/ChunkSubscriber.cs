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
            consumer.On<Moved>((source, _) =>
            {
                var tracker = trackers.GetOrAdd(source, _ => new Tracker());
                var center = (X: (int) Math.Floor(source.Position.X) >> 4, Z: (int) Math.Floor(source.Position.Z) >> 4);

                if (tracker.World != source.World)
                {
                    // The client clears its chunks when changing worlds, so they must all be sent again.
                    tracker.World = source.World;
                    tracker.Loaded.Clear();
                }
                else if (tracker.Center == center && tracker.Distance == source.ViewDistance)
                {
                    return;
                }

                tracker.Center = center;
                tracker.Distance = source.ViewDistance;

                var alive = new HashSet<(int X, int Z)>();

                for (var x = center.X - source.ViewDistance; x <= center.X + source.ViewDistance; x++)
                {
                    for (var z = center.Z - source.ViewDistance; z <= center.Z + source.ViewDistance; z++)
                    {
                        alive.Add((x, z));
                    }
                }

                foreach (var dead in tracker.Loaded.Where(chunk => !alive.Contains(chunk)).ToArray())
                {
                    source.Client.Write(new SingleChunkPacket(dead.X, dead.Z, [], 0));
                    tracker.Loaded.Remove(dead);
                }

                var closest = alive.OrderBy(chunk => (chunk.X - center.X) * (chunk.X - center.X) + (chunk.Z - center.Z) * (chunk.Z - center.Z));

                foreach (var chunk in closest)
                {
                    if (tracker.Loaded.Add(chunk))
                    {
                        source.Client.Write(source.World[chunk.X, chunk.Z].Build());
                    }
                }
            });

            consumer.On<Left>((source, _) => trackers.TryRemove(source, out var tracker));
        });
    }

    private sealed class Tracker
    {
        public IWorld? World { get; set; }

        public (int X, int Z) Center { get; set; }

        public byte Distance { get; set; }

        public HashSet<(int X, int Z)> Loaded { get; } = [];
    }
}