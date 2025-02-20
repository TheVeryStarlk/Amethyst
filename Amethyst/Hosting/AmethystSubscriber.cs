using System.Numerics;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Sources.Clients;
using Amethyst.Abstractions.Eventing.Sources.Players;
using Amethyst.Abstractions.Worlds;
using Amethyst.Protocol.Packets.Play.Entities;
using Amethyst.Protocol.Packets.Play.Players;
using Amethyst.Protocol.Packets.Play.Worlds;
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
                source.Stop("Already logged in.");
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                loaded[source.Username] = [];

                var world = (World) source.World;
                world.AddPlayer(source);

                var action = new AddPlayerAction();

                foreach (var player in world.Players.Values)
                {
                    source.Client.Write(new ListItemPacket(action, player));
                    player.Client.Write(new ListItemPacket(action, source));

                    if (player == source)
                    {
                        continue;
                    }

                    source.Client.Write(new SpawnPlayerPacket(player));
                    player.Client.Write(new SpawnPlayerPacket(source));
                }
            });

            consumer.On<Left>((source, _) =>
            {
                loaded.Remove(source.Username);

                var world = (World) source.World;
                world.RemovePlayer(source);

                var action = new RemovePlayerAction();

                foreach (var player in world.Players.Values)
                {
                    player.Client.Write(new ListItemPacket(action, source));
                    player.Client.Write(new DestroyEntitiesPacket(source));
                }
            });

            consumer.On<Moved>((source, original) =>
            {
                foreach (var player in source.World.Players.Values.Where(player => player.Username != source.Username))
                {
                    player.Client.Write(
                        new EntityLookRelativeMovePacket(source, (original.Location - source.Location).ToAbsolute()),
                        new EntityHeadLook(source));
                }

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

                    var result = world.GetChunk(x, z).Build();
                    source.Client.Write(new SingleChunkPacket(x, z, result.Sections, result.Bitmask));
                }
            });
        });
    }
}