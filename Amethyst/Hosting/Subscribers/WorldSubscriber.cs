using System.Numerics;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Worlds;
using Amethyst.Protocol.Packets.Play.Players;
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

            consumer.On<Digging>((source, original) =>
            {
                var packet = new BlockChangePacket(original.Position, new Block(0));

                foreach (var player in source.World.Players.Values.Where(player => player != source))
                {
                    player.Client.Write(packet);
                }

                source.World.SetBlock(new Block(0), original.Position);
            });

            consumer.On<Placing>((source, original) =>
            {
                var position = original.Face switch
                {
                    BlockFace.NegativeY => original.Position with { Y = original.Position.Y - 1 },
                    BlockFace.PositiveY => original.Position with { Y = original.Position.Y + 1 },
                    BlockFace.NegativeZ => original.Position with { Z = original.Position.Z - 1 },
                    BlockFace.PositiveZ => original.Position with { Z = original.Position.Z + 1 },
                    BlockFace.NegativeX => original.Position with { X = original.Position.X - 1 },
                    BlockFace.PositiveX => original.Position with { X = original.Position.X + 1 },
                    _ => throw new ArgumentException("Unknown face.")
                };

                var packet = new BlockChangePacket(position, new Block(1));

                foreach (var player in source.World.Players.Values)
                {
                    player.Client.Write(packet);
                }

                source.World.SetBlock(new Block(1), position);
            });

            consumer.On<Moved>((source, _) =>
            {
                var current = source.Location.ToPosition().ToChunk();

                var length = (current.X - source.ViewDistance - (current.X + source.ViewDistance))
                             * (current.Z - source.ViewDistance - (current.Z + source.ViewDistance));

                var alive = new long[length];
                var index = 0;

                for (var x = current.X - source.ViewDistance; x < current.X + source.ViewDistance; x++)
                {
                    for (var z = current.Z - source.ViewDistance; z < current.Z + source.ViewDistance; z++)
                    {
                        alive[index++] = NumericHelper.Encode(x, z);
                    }
                }

                var chunks = loaded[source.Username];
                var dead = chunks.Except(alive).ToArray();

                foreach (var value in dead)
                {
                    NumericHelper.Decode(value, out var x, out var z);

                    source.Client.Write(new SingleChunkPacket(x, z, [], 0));
                    chunks.Remove(value);
                }

                var closest = alive.OrderBy(value =>
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