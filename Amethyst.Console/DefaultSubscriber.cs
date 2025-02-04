using Amethyst.Components;
using Amethyst.Components.Messages;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Sources.Clients;
using Amethyst.Eventing.Sources.Players;
using Amethyst.Eventing.Sources.Servers;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber(AuthenticationService authenticationService) : ISubscriber
{
    private readonly Dictionary<string, Player> players = [];
    private readonly Message bye = Message.Create().Write("You're scaring me!").Red().Build();

    public void Subscribe(IRegistry registry)
    {
        registry.For<Server>(consumer => consumer.On<StatusRequest>((_, request, _) =>
        {
            var description = Message
                .Create()
                .WriteLine("Hello, world!").Bold()
                .Write("Powered by ").Gray()
                .Write("Amethyst").LightPurple()
                .Build();

            request.Status = Status.Create("Amethyst", 47, players.Count + 1, players.Count, description, string.Empty);
            return Task.CompletedTask;
        }));

        registry.For<Client>(consumer => consumer.On<Outdated>((_, outdated, _) =>
        {
            outdated.Message = bye;
            return Task.CompletedTask;
        }));

        registry.For<Player>(consumer =>
        {
            consumer.On<Joined>(async (player, _, _) =>
            {
                players[player.Username] = player;

                var message = Message.Create("Enter your password.", color: Color.Red);
                await player.SendAsync(message, MessagePosition.Box);
            });

            consumer.On<Received>(async (player, received, _) =>
            {
                if (received.Packet.Identifier != MessagePacket.Identifier)
                {
                    return;
                }

                var packet = received.Packet.Create<MessagePacket>();

                if (authenticationService.TryAuthenticate(packet.Message))
                {
                    await player.SendAsync(packet.Message, MessagePosition.Box);
                    return;
                }

                var message = Message.Create("Incorrect password.", color: Color.Red);
                player.Disconnect(message);
            });

            consumer.On<Left>((player, _, _) =>
            {
                players.Remove(player.Username);
                return Task.CompletedTask;
            });
        });
    }
}