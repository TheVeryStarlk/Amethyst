using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Worlds;
using Amethyst.Entities;
using Amethyst.Worlds;

namespace Amethyst.Hosting;

using Vector = (int X, int Z);

internal sealed class AmethystSubscriber : ISubscriber
{
    private readonly List<Vector> chunks = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _, _) =>
            {
                source.Spawn(new World("Funny"));
                return Task.CompletedTask;
            });

            consumer.On<Sent>((source, _, _) =>
            {
                source.Spawn(new World("Funny"));
                return Task.CompletedTask;
            });

            consumer.On<Moved>((source, moved, _) =>
            {
                // We need to access the internal class to update the player's properties.
                var player = (Player) source;

                player.Location = moved.Location;
                player.Yaw = moved.Yaw;
                player.Pitch = moved.Pitch;
                player.OnGround = moved.OnGround;

                var vector = new Vector((int) player.Location.X >> 4, (int) player.Location.Z >> 4);

                if (chunks.Contains(vector) || chunks.Count >= 32)
                {
                    return Task.CompletedTask;
                }

                var chunk = new Chunk(vector.X, vector.Z);

                for (var x = 0; x < 16; x++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        chunk.SetBlock(new Block(35, Random.Shared.Next(15)), new Position(x, 2, z));
                    }
                }

                chunks.Add(vector);

                player.Client.Write(chunk.Build());

                return Task.CompletedTask;
            });
        });
    }
}