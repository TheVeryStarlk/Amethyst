using Amethyst.Components;
using Amethyst.Components.Messages;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Sources.Players;
using Amethyst.Eventing.Sources.Servers;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber : ISubscriber
{
    private readonly Dictionary<string, Player> players = [];

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

            request.Status = Status.Create("Amethyst", 47, players.Count, players.Count, description, string.Empty);
            return Task.CompletedTask;
        }));

        registry.For<Player>(consumer =>
        {
            consumer.On<Moved>(async (player, _, _) =>
            {
                await player.MoveAsync(0, 0, 0, 0, 0);
            });

            consumer.On<Joined>(async (player, _, _) =>
            {
                players[player.Username] = player;

                var message = Message.Create($"Welcome {player.Username}!", color: Color.Green);
                await player.SendAsync(message);
            });

            consumer.On<Left>((player, _, _) =>
            {
                players.Remove(player.Username);
                return Task.CompletedTask;
            });
        });
    }
}