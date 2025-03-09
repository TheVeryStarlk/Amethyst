using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Eventing.Clients;
using Amethyst.Eventing.Players;
using Amethyst.Eventing.Servers;
using Amethyst.Extensions.Commands;

namespace Amethyst.Example;

internal sealed class DefaultSubscriber : ISubscriber
{
    private readonly CommandsEngine commandsEngine = CommandsEngine
        .Create()
        .Map("kick", (player, arguments) => player.Disconnect(arguments[0]))
        .Failure((player, _) => player.Send(Message.Create("Incorrect usage!", color: Color.Red)))
        .Build();

    private IWorldStore? worldStore;

    public void Subscribe(Registry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Starting>((source, _) => worldStore = source.WorldStore));

        registry.For<IClient>(consumer => consumer.On<Joining>((_, original) =>
        {
            const string name = "Default";

            if (!worldStore!.Worlds.ContainsKey(name))
            {
                worldStore.Create("Default", new FlatGenerator());
            }

            original.World = worldStore!.Worlds["Default"];
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) => source.Teleport(new Location(0, 8, 0)));
            consumer.On<Tab>((_, original) => original.Matches = commandsEngine.Registered);

            consumer.On<Sent>((source, original) =>
            {
                if (commandsEngine.TryExecute(source, original.Message))
                {
                    return;
                }

                foreach (var world in worldStore!.Worlds)
                {
                    foreach (var player in world.Value.Players)
                    {
                        var message = Message
                            .Create()
                            .Write($"{world.Key} - {player.Key}: ").Gray()
                            .Write(original.Message)
                            .Build();

                        player.Value.Send(message);
                    }
                }
            });
        });
    }
}