using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Eventing.Sources.Servers;
using Amethyst.Components.Messages;
using Amethyst.Components.Worlds;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger, IPlayerStore playerStore, IWorldStore worldStore) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Starting>((_, _) => worldStore.Create("Default", new FlatGenerator())));
        registry.For<IClient>(consumer => consumer.On<Joining>((_, joining) => joining.World = worldStore["Default"]));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Sent>((source, sent) =>
            {
                logger.LogInformation("Broadcasting: \"{Message}\"", sent.Message);

                var message = Message
                    .Create()
                    .Write($"{source.Username}: ").Gray()
                    .Write(sent.Message)
                    .Build();

                foreach (var player in playerStore)
                {
                    player.Send(message);
                }
            });
        });
    }
}