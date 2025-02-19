using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Eventing.Sources.Servers;
using Amethyst.Components.Messages;
using Amethyst.Components.Worlds;
using Amethyst.Extensions.Commands;

namespace Amethyst.Example;

internal sealed class DefaultSubscriber(IWorldStore worldStore) : ISubscriber
{
    private readonly CommandsEngine commandsEngine = CommandsEngine
        .Create()
        .Map("kick", (player, arguments) => player.Disconnect(arguments[0]))
        .Failure((player, _) => player.Send(Message.Create("Incorrect usage!", color: Color.Red)))
        .Build();

    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Starting>((_, _) => worldStore.Create("Default", new FlatGenerator())));
        registry.For<IClient>(consumer => consumer.On<Joining>((_, original) => original.World = worldStore["Default"]));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) => source.Teleport(new Location(0, 32, 0)));

            consumer.On<Tab>((_, original) =>
            {
                Console.WriteLine(original.Behind);
            });

            consumer.On<Sent>((source, original) =>
            {
                if (commandsEngine.TryExecute(source, original.Message))
                {
                    return;
                }

                var message = Message
                    .Create()
                    .Write($"{source.Username}: ").Gray()
                    .Write(original.Message)
                    .Build();

                foreach (var player in source.World.Players)
                {
                    player.Value.Send(message);
                }
            });
        });
    }
}