using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Worlds;
using Amethyst.Entities;
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
                source.Stop("Bad!");
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) => ((World) source.World).AddPlayer(source));

            consumer.On<Left>((source, _) =>
            {
                ((World) source.World).RemovePlayer(source);
                loaded.Remove(source.Username);
            });

            consumer.On<Configuration>((source, original) =>
            {
                var player = (Player) source;

                if (player.ViewDistance == original.ViewDistance)
                {
                    return;
                }

                player.ViewDistance = original.ViewDistance;
                loaded[player.Username] = [];
            });

            consumer.On<Moved>((source, _) =>
            {
                if (!loaded.TryGetValue(source.Username, out var chunks))
                {
                    chunks = [];
                    loaded[source.Username] = chunks;
                }

                var world = (World) source.World;

                var current = new Position((int) source.Location.X, 0, (int) source.Location.Z).ToChunk();
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

                foreach (var value in temporary)
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