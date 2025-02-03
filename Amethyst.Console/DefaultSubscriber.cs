using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Sources.Client;
using Amethyst.Abstractions.Eventing.Sources.Player;
using Amethyst.Abstractions.Eventing.Sources.Server;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer =>
        {
            consumer.On<Stopping>((_, stopping, _) =>
            {
                stopping.Message = Message.Create("Come back later!");
                return Task.CompletedTask;
            });
        });

        registry.For<IClient>(consumer =>
        {
            consumer.On<Outdated>((_, outdated, _) =>
            {
                // Eh, should probably cache this.
                var message = Message
                    .Create()
                    .Write("You're scaring me!").Red()
                    .Build();

                outdated.Message = message;
                return Task.CompletedTask;
            });

            consumer.On<StatusRequest>((_, request, _) =>
            {
                var description = Message
                    .Create()
                    .WriteLine("Hello, world!").Bold()
                    .Write("Powered by ").Gray()
                    .Write("Amethyst").LightPurple()
                    .Build();

                request.Status = Status.Create("Amethyst", 47, 1, 0, description, string.Empty);
                return Task.CompletedTask;
            });
        });

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>(async (player, _, _) =>
            {
                await player.SendAsync("Welcome.", 0);
                await player.SendAsync("Hey there.", 1);
                await player.SendAsync("Why hello!", 2);
            });
        });
    }
}