using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;
using Amethyst.Eventing.Server;

namespace Amethyst.Hosting.Subscribing;

internal sealed class AmethystSubscriber(PlayerRepository playerRepository) : ISubscriber
{
    private readonly FailurePacket failure = new(Message.Simple("Bad username!"));

    private DateTime last;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Joining>((source, original) =>
        {
            if (playerRepository.Players.ContainsKey(original.Username))
            {
                source.Write(failure);
            }
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                playerRepository.Add(source);

                var list = new ListItemPacket(
                    new AddPlayerAction(source.Username, source.GameMode, 0, Message.Simple(source.Username)),
                    source);

                var spawn = new SpawnPlayerPacket(source);

                foreach (var pair in playerRepository.Players)
                {
                    pair.Value.Client.Write(list);

                    source.Client.Write(new ListItemPacket(
                        new AddPlayerAction(pair.Value.Username, pair.Value.GameMode, 0, Message.Simple(pair.Value.Username)),
                        pair.Value));

                    if (pair.Value == source || pair.Value.World != source.World)
                    {
                        continue;
                    }

                    pair.Value.Client.Write(spawn);
                    source.Client.Write(new SpawnPlayerPacket(pair.Value));
                }
            });

            consumer.On<Left>((source, _) =>
            {
                playerRepository.Remove(source);

                var packet = new ListItemPacket(new RemovePlayerAction(), source);

                foreach (var pair in playerRepository.Players)
                {
                    pair.Value.Client.Write(packet);
                }
            });
        });

        // How about removing the idea of ticking?
        registry.For<IServer>(consumer => consumer.On<Tick>((_, _) =>
        {
            var now = DateTime.Now;

            if (now - last < TimeSpan.FromSeconds(5))
            {
                return;
            }

            last = now;

            var alive = new KeepAlivePacket((int) now.Ticks);

            foreach (var pair in playerRepository.Players)
            {
                pair.Value.Client.Write(alive);
            }
        }));
    }
}