using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Messages;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer => consumer.On<Sent>((player, sent, _) =>
        {
            var message = Message
                .Create()
                .Write($"{player.Username}: ").Gray()
                .Write(sent.Message)
                .Build();

            player.Send(message);
            return Task.CompletedTask;
        }));
    }
}