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

                foreach (var pair in source.World.Players)
                {
                    if (pair.Value == source)
                    {
                        continue;
                    }

                    pair.Value.Client.Write(spawn);
                    source.Client.Write(new SpawnPlayerPacket(pair.Value));
                }
            });

            consumer.On<Moved>((source, _) =>
            {
                // Use relative movement packets.
                var packet = new EntityTeleportPacket(source);

                foreach (var pair in source.World.Players)
                {
                    if (pair.Value != source)
                    {
                        pair.Value.Client.Write(packet);
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
