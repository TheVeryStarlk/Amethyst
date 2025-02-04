using Amethyst.Components;
using Amethyst.Components.Messages;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Sources.Clients;
using Amethyst.Eventing.Sources.Players;
using Amethyst.Eventing.Sources.Servers;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<Server>(consumer =>
        {
            consumer.On<Stopping>((_, stopping, _) =>
            {
                stopping.Message = Message.Create("Come back later!");
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

                request.Status = Status.Create("Amethyst", 47, 0, 0, description, string.Empty);
                return Task.CompletedTask;
            });
        });

        registry.For<Client>(consumer =>
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
        });

        registry.For<Player>(consumer =>
        {
            consumer.On<Joined>(async (player, _, _) =>
            {
                await player.Client.WriteAsync(new DisconnectPacket("aaa"));
                var message = Message.Create("Welcome!", color: Color.Yellow);
                await player.SendAsync(message, MessagePosition.Box);
            });
        });
    }
}