using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Worlds;
using Amethyst.Entities;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Worlds;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Sent>((source, _, _) =>
            {
                var world = new World("Funny");

                for (var x = -64; x < 64; x++)
                {
                    for (var z = -64; z < 64; z++)
                    {
                        world.SetBlock(new Block(1), new Position(x, 2, z));
                    }
                }

                foreach (var region in world.Regions)
                {
                    foreach (var chunk in region.Chunks.OfType<Chunk>())
                    {
                        source.Client.Write(chunk.Build());
                    }
                }

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

                return Task.CompletedTask;
            });
        });
    }
}