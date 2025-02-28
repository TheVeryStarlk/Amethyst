using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Clients;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Protocol.Packets.Play.Entities;
using Amethyst.Protocol.Packets.Play.Players;
using Amethyst.Worlds;

namespace Amethyst.Hosting.Subscribers;

internal sealed class PlayerSubscriber(WorldStore worldStore) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Joining>((source, original) =>
        {
            if (worldStore.Any(worlds => worlds.Value.Any(play => play.Key == original.Username)))
            {
                // Does this need to be customizable?
                source.Stop("Already logged in.");
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                worldStore.Add(source);

                var action = new AddPlayerAction();

                foreach (var player in source.World.Players.Values)
                {
                    source.Client.Write(new ListItemPacket(action, player));
                    player.Client.Write(new ListItemPacket(action, source));

                    if (player == source)
                    {
                        continue;
                    }

                    source.Client.Write(new SpawnPlayerPacket(player));
                    player.Client.Write(new SpawnPlayerPacket(source));
                }
            });

            consumer.On<Left>((source, _) =>
            {
                worldStore.Remove(source);

                var action = new RemovePlayerAction();

                foreach (var player in source.World.Players.Values)
                {
                    player.Client.Write(new ListItemPacket(action, source));
                    player.Client.Write(new DestroyEntitiesPacket(source));
                }
            });

            consumer.On<Moved>((source, original) =>
            {
                foreach (var player in source.World.Players.Values.Where(player => player.Username != source.Username))
                {
                    player.Client.Write(
                        new EntityLookRelativeMovePacket(source, (original.Location - source.Location).ToAbsolute()),
                        new EntityHeadLook(source));
                }
            });
        });
    }
}