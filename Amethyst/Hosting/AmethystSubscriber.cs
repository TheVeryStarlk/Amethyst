using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Worlds;
using Amethyst.Entities;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Worlds;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber(IPlayerStore store) : ISubscriber
{
    private readonly PlayerStore playerStore = (PlayerStore) store;
    private readonly List<Position> chunks = [];

    private int range = 2;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                if (!playerStore.TryAdd(source))
                {
                    source.Disconnect("Joined from another location.");
                }
            });

            consumer.On<Left>((source, _) => playerStore.Remove(source));

            consumer.On<Sent>((source, original) =>
            {
                range = int.Parse(original.Message);

                foreach (var chunk in chunks)
                {
                    source.Client.Write(new ChunkUnloadPacket(chunk.X, chunk.Z));
                }

                chunks.Clear();
            });

            consumer.On<Moved>((source, _) =>
            {
                var current = new Position((int) source.Location.X >> 4, 0, (int) source.Location.Z >> 4);

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

                    var chunk = new Chunk(position.X, position.Z);

                    for (var x = 0; x < 16; x++)
                    {
                        for (var z = 0; z < 16; z++)
                        {
                            chunk.SetBlock(new Block(1), new Position(x, 2, z));
                        }
                    }

                    source.Client.Write(chunk.Build());
                }
            });
        });
    }
}