using System.Numerics;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;
using Amethyst.Eventing.Server;
using Amethyst.Worlds;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber : ISubscriber
{
    private readonly Dictionary<string, IPlayer> players = [];
    private readonly Dictionary<string, HashSet<long>> chunks = [];
    private readonly FailurePacket failure = new(Message.Simple("Bad username!"));

    private DateTime last;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Joining>((source, original) =>
        {
            if (players.ContainsKey(original.Username))
            {
                source.Write(failure);
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                players[source.Username] = source;
                chunks[source.Username] = [];
            });

            consumer.On<Left>((source, _) =>
            {
                players.Remove(source.Username);
                chunks.Remove(source.Username);
            });

            consumer.On<Moved>((source, original) =>
            {
                var alive = new long[source.ViewDistance * source.ViewDistance * 4];
                var index = 0;

                var position = (X: ((int) original.Position.X).ToChunk(), Z: ((int) original.Position.Z).ToChunk());

                for (var x = position.X - source.ViewDistance; x < position.X + source.ViewDistance; x++)
                {
                    for (var z = position.Z - source.ViewDistance; z < position.Z + source.ViewDistance; z++)
                    {
                        alive[index++] = NumericUtility.Encode(x, z);
                    }
                }

                var used = chunks[source.Username];
                var dead = used.Except(alive).ToArray();

                foreach (var value in dead)
                {
                    value.Decode(out var x, out var z);

                    source.Client.Write(new SingleChunkPacket(x, z, [], 0));
                    used.Remove(value);
                }

                var closest = alive.OrderBy(value =>
                {
                    value.Decode(out var x, out var z);
                    return Vector2.Distance(new Vector2(position.X, position.Z), new Vector2(x, z));
                });

                foreach (var value in closest)
                {
                    if (!used.Add(value))
                    {
                        continue;
                    }

                    value.Decode(out var x, out var z);

                    var result = source.World[x, z].Build();
                    source.Client.Write(new SingleChunkPacket(x, z, result.Sections, result.Bitmask));
                }
            });
        });

        // How about removing the idea of ticking?
        registry.For<IServer>(consumer => consumer.On<Tick>((_, _) =>
        {
            var now = DateTime.Now;

            // Make keep alive interval be configurable?
            if (now - last < TimeSpan.FromSeconds(5))
            {
                return;
            }

            last = now;

            var alive = new KeepAlivePacket((int) now.Ticks);

            foreach (var pair in players)
            {
                pair.Value.Client.Write(alive);
            }
        }));
    }
}