using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Worlds;
using Amethyst.Entities;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Worlds;

namespace Amethyst.Hosting;

internal sealed class InternalSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Sent>((source, _, _) =>
            {
                var chunks = new Chunk[]
                {
                    new(0, 0),
                    new(-1, 0),
                    new(-1, -1),
                    new(0, -1),
                };

                foreach (var chunk in chunks)
                {
                    for (var x = 0; x < 16; x++)
                    {
                        for (var z = 0; z < 16; z++)
                        {
                            chunk.SetBlock(new Block(1), x, 4, z);
                        }
                    }

                    var build = chunk.Build();
                    source.Client.Write(new ChunkPacket(chunk.X, chunk.Z, build.Buffer, build.Bitmask));
                }

                return Task.CompletedTask;
            });

            consumer.On<Moved>((source, moved, _) =>
            {
                // We need to access the internal class to update the player's properties.
                var player = (Player) source;

                player.X = moved.X;
                player.Y = moved.Y;
                player.Z = moved.Z;
                player.Yaw = moved.Yaw;
                player.Pitch = moved.Pitch;
                player.OnGround = moved.OnGround;

                return Task.CompletedTask;
            });
        });
    }
}