using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
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
                var chunk = new Chunk(0, 0);

                chunk.SetBlock(new Block(1), 4, 4, 4);

                var build = chunk.Build();

                var packet = new ChunkPacket(0, 0, build.Buffer, build.Bitmask);

                source.Client.Write(packet);

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