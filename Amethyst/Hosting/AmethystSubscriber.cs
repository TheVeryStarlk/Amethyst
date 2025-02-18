using System.Numerics;
using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Worlds;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Worlds;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber(IWorldStore worldStore) : ISubscriber
{
    private readonly Dictionary<string, HashSet<long>> loaded = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Joining>((source, original) =>
        {
            if (worldStore.Any(world => world.Players.Any(pair => pair.Key == original.Username)))
            {
                // Does this need to be customizable?
                source.Stop("Logged in from another location.");
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                ((World) source.World).AddPlayer(source);
                loaded[source.Username] = [];
            });

            consumer.On<Left>((source, _) =>
            {
                ((World) source.World).RemovePlayer(source);
                loaded.Remove(source.Username);
            });

            consumer.On<Configuration>((source, _) =>
            {
                var chunks = loaded[source.Username];

                foreach (var value in chunks)
                {
                    NumericHelper.Decode(value, out var x, out var z);
                    source.Client.Write(new ChunkUnloadPacket(x, z));
                }

                chunks.Clear();
            });

            consumer.On<Moved>((source, _) =>
            {
                var chunks = loaded[source.Username];
                var world = (World) source.World;

                var current = source.Location.ToPosition().ToChunk();
                var temporary = new List<long>();

                for (var x = current.X - source.ViewDistance; x < current.X + source.ViewDistance; x++)
                {
                    for (var z = current.Z - source.ViewDistance; z < current.Z + source.ViewDistance; z++)
                    {
                        temporary.Add(NumericHelper.Encode(x, z));
                    }
                }

                var dead = chunks.Except(temporary).ToArray();

                foreach (var value in dead)
                {
                    NumericHelper.Decode(value, out var x, out var z);

                    source.Client.Write(new ChunkUnloadPacket(x, z));
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

                    var result = world.GetChunk(x, z).Build();
                    source.Client.Write(new SingleChunkPacket(x, z, result.Sections, result.Bitmask));
                }
            });
        });
    }
}