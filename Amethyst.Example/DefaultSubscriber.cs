using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Eventing.Sources.Servers;
using Amethyst.Components.Messages;
using Amethyst.Components.Worlds;

namespace Amethyst.Example;

internal sealed class DefaultSubscriber(IPlayerStore playerStore, IWorldStore worldStore) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Starting>((_, _) => worldStore.Create("Default", new FlatGenerator())));
        registry.For<IClient>(consumer => consumer.On<Joining>((_, original) => original.World = worldStore["Default"]));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) => source.Teleport(new Location(0, 8, 0)));

            consumer.On<Sent>((source, original) =>
            {
                var message = Message
                    .Create()
                    .Write($"{source.Username}: ").Gray()
                    .Write(original.Message)
                    .Build();

                foreach (var player in playerStore)
                {
                    player.Send(message);
                }
            });
        });
    }
}