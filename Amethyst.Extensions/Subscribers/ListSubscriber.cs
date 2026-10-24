using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Keeps every player's tab list in sync with the players in the server.
/// </summary>
public sealed class ListSubscriber(IWorldFactory worldFactory) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                var list = new ListItemPacket(
                    new AddPlayerAction(
                        source.Username,
                        source.GameMode, 
                        0, 
                        Message.Simple(source.Username)),
                    source);

                foreach (var player in worldFactory.Worlds.SelectMany(world => world.Players.Values))
                {
                    player.Client.Write(list);

                    if (player == source)
                    {
                        continue;
                    }

                    source.Client.Write(new ListItemPacket(
                        new AddPlayerAction(
                            player.Username,
                            player.GameMode, 
                            0, 
                            Message.Simple(player.Username)),
                        player));
                }
            });

            consumer.On<Left>((source, _) =>
            {
                var list = new ListItemPacket(new RemovePlayerAction(), source);

                foreach (var player in worldFactory.Worlds.SelectMany(world => world.Players.Values))
                {
                    player.Client.Write(list);
                }
            });
        });
    }
}