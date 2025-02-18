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

internal sealed class AmethystSubscriber(IPlayerStore store) : ISubscriber
{
    private readonly PlayerStore playerStore = (PlayerStore) store;
    private readonly Dictionary<string, List<Position>> loaded = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Joining>((source, original) =>
        {
            if (playerStore.Any(player => player.Username == original.Username))
            {
                source.Stop("Bad!");
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) => playerStore.Add(source));
            consumer.On<Left>((source, _) => playerStore.Remove(source));

            consumer.On<Moved>((source, _) =>
            {
                if (!loaded.TryGetValue(source.Username, out var chunks))
                {
                    chunks = [];
                    loaded[source.Username] = chunks;
                }

                var world = (World) source.World;

                // Should be configurable via client packets.
                const int range = 2;

                var current = source.Location.ToPosition().ToChunk();
                var temporary = new List<Position>();

                for (var x = current.X - range; x < current.X + range; x++)
                {
                    for (var z = current.Z - range; z < current.Z + range; z++)
                    {
                        temporary.Add(new Position(x, 0, z));
                    }
                }

                var dead = chunks.Except(temporary).ToArray();

                foreach (var position in dead)
                {
                    source.Client.Write(new ChunkUnloadPacket(position.X, position.Z));
                    chunks.Remove(position);
                }

                foreach (var position in temporary.Where(position => !chunks.Contains(position)))
                {
                    chunks.Add(position);

                    var chunk = world.GetChunk(position);
                    var result = chunk.Build();

                    source.Client.Write(new SingleChunkPacket(chunk.Position.X, chunk.Position.Z, result.Chunk, result.Bitmask));
                }
            });
        });
    }
}