using System.Numerics;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Protocol.Packets.Play.Worlds;
using Amethyst.Worlds;

namespace Amethyst.Hosting.Subscribers;

internal sealed class WorldSubscriber : ISubscriber
{
    private readonly Dictionary<string, HashSet<long>> loaded = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) => loaded[source.Username] = []);
            consumer.On<Left>((source, _) => loaded.Remove(source.Username));

            consumer.On<Moved>((source, _) =>
            {
                var current = source.Location.ToPosition().ToChunk();
                var temporary = new List<long>();

                for (var x = current.X - source.ViewDistance; x < current.X + source.ViewDistance; x++)
                {
                    for (var z = current.Z - source.ViewDistance; z < current.Z + source.ViewDistance; z++)
                    {
                        temporary.Add(NumericHelper.Encode(x, z));
                    }
                }

                var chunks = loaded[source.Username];
                var dead = chunks.Except(temporary).ToArray();

                foreach (var value in dead)
                {
                    NumericHelper.Decode(value, out var x, out var z);

                    source.Client.Write(new SingleChunkPacket(x, z, [], 0));
                    chunks.Remove(value);
                }

                var closest = temporary.OrderBy(value =>
                {
                    NumericHelper.Decode(value, out var x, out var z);
                    return Vector2.Distance(new Vector2(current.X, current.Z), new Vector2(x, z));
                });

                foreach (var value in closest)
                {
                    if (!chunks.Add(value))
                    {
                        continue;
                    }

                    NumericHelper.Decode(value, out var x, out var z);

                    var result = source.World.GetChunk(x, z).Build();
                    source.Client.Write(new SingleChunkPacket(x, z, result.Sections, result.Bitmask));
                }
            });
        });
    }
}