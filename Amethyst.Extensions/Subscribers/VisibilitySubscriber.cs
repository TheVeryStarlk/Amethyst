using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Spawns, moves and despawns players for each other when they share a world.
/// </summary>
public sealed class VisibilitySubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                var spawn = new SpawnPlayerPacket(source);
                var look = new EntityHeadLookPacket(source);

                foreach (var pair in source.World.Players)
                {
                    if (pair.Value == source)
                    {
                        continue;
                    }

                    // Spawning only rotates the body, the head has to be turned separately.
                    pair.Value.Client.Write(spawn, look);
                    source.Client.Write(new SpawnPlayerPacket(pair.Value), new EntityHeadLookPacket(pair.Value));
                }
            });

            consumer.On<Moved>((source, _) =>
            {
                // Use relative movement packets.
                var teleport = new EntityTeleportPacket(source);
                var look = new EntityHeadLookPacket(source);

                foreach (var pair in source.World.Players)
                {
                    if (pair.Value != source)
                    {
                        pair.Value.Client.Write(teleport, look);
                    }
                }
            });

            consumer.On<Left>((source, _) =>
            {
                var destroy = new DestroyEntitiesPacket(source);

                foreach (var pair in source.World.Players)
                {
                    pair.Value.Client.Write(destroy);
                }
            });
        });
    }
}
