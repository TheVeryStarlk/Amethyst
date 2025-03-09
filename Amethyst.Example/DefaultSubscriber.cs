using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing.Clients;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Eventing.Servers;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Extensions.Commands;

namespace Amethyst.Example;

internal sealed class DefaultSubscriber : ISubscriber
{
    private readonly CommandsEngine commandsEngine = CommandsEngine
        .Create()
        .Map("kick", (player, arguments) => player.Disconnect(arguments[0]))
        .Failure((player, _) => player.Send(Message.Create("Incorrect usage!", color: Color.Red)))
        .Build();

    private IWorldService? worldService;

    public void Subscribe(Registry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Starting>((source, _) =>
        {
            worldService = source.WorldManager;
            source.WorldManager.Create("Default", new FlatGenerator());
        }));

        registry.For<IClient>(consumer => consumer.On<Joining>((_, original) => original.World = worldService!.Worlds["Default"]));

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

                foreach (var world in worldService!.Worlds)
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