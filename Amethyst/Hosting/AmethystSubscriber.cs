using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber(IWorldFactory worldFactory) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                var list = new ListItemPacket(
                    new AddPlayerAction(source.Username, source.GameMode, 0, Message.Simple(source.Username)),
                    source);

                var spawn = new SpawnPlayerPacket(source);

                foreach (var pair in Players())
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
                var list = new ListItemPacket(new RemovePlayerAction(), source);
                var destroy = new DestroyEntitiesPacket(source);

                foreach (var pair in Players())
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

    private IEnumerable<KeyValuePair<string, IPlayer>> Players()
    {
        return worldFactory.Worlds.SelectMany(world => world.Players);
    }
}
