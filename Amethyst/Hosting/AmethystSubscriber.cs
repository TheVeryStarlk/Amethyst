using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities.Players;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber(PlayerRepository playerRepository) : ISubscriber
{
    private readonly FailurePacket failure = new(Message.Simple("Bad username!"));

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

                var list = new ListItemPacket(new RemovePlayerAction(), source);
                var destroy = new DestroyEntitiesPacket(source);

                foreach (var pair in playerRepository.Players)
                {
                    pair.Value.Client.Write(list);

                    if (pair.Value.World == source.World)
                    {
                        pair.Value.Client.Write(destroy);
                    }
                }
            });
        });
    }
}