using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Messages;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger) : ISubscriber
{
    private readonly Dictionary<string, IPlayer> players = [];

    private const string Icon =
        "data:image/png;base64,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";

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Request>((_, request, _) =>
        {
            request.Status = Status.Create("Amethyst", 47, players.Count, players.Count, Message.Create("Hello, world!"), Icon);
            return Task.CompletedTask;
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((player, _, _) =>
            {
                player.Disconnect("Bad!");

                // players[player.Username] = player;
                return Task.CompletedTask;
            });

            consumer.On<Left>((player, _, _) =>
            {
                if (!players.Remove(player.Username))
                {
                    logger.LogWarning("Failed to remove a player");
                }

                return Task.CompletedTask;
            });
        });
    }
}