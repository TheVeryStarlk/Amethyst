using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Eventing.Server;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Extensions;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example;

internal sealed class ExampleSubscriber(ILogger<ExampleSubscriber> logger, IWorldFactory worldFactory) : ISubscriber
{
    private static readonly Position spawn = new(0.5, FlatGenerator.Height, 0.5);

    public void Subscribe(IRegistry registry)
    {
        // Create the world once, players are tracked by the world they are in.
        var world = worldFactory.Create("Flat", new FlatGenerator(), WorldType.Flat);

        registry.For<IServer>(consumer =>
        {
            consumer.On<Starting>((_, starting) => logger.LogInformation("Listening on {EndPoint}", starting.EndPoint));
            consumer.On<Stopping>((_, stopping) => stopping.Message = Message.Simple("The example server is shutting down."));
        });

        registry.For<IClient>(consumer =>
        {
            consumer.On<Request>((_, request) =>
            {
                request.Name = "Amethyst 1.8.9";
                request.Online = worldFactory.Worlds.Sum(instance => instance.Players.Count);
                request.Description = Message.Create().Write("Amethyst ").LightPurple().Write("example server").Build();
            });

            consumer.On<Outdated>((_, outdated) => outdated.Message = Message.Simple("Please join using Minecraft 1.8.9."));

            consumer.On<Joining>((_, joining) =>
            {
                joining.World = world;
                joining.GameMode = GameMode.Creative;
            });
        });

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((player, _) =>
            {
                // Players are placed at the origin on join, move them on top of the ground.
                player.Teleport(spawn);
                player.Send(Message.Create().Write("Welcome ").Gray().Write(player.Username).Write("! Type /help for commands.").Gray().Build());

                Broadcast(player.World, Message.Create().Write($"{player.Username} joined the game.").Yellow().Build());
                logger.LogInformation("{Username} joined", player.Username);
            });

            consumer.On<Left>((player, _) =>
            {
                Broadcast(player.World, Message.Create().Write($"{player.Username} left the game.").Yellow().Build());
                logger.LogInformation("{Username} left", player.Username);
            });

            consumer.On<Sent>((player, sent) =>
            {
                if (sent.Message.StartsWith('/'))
                {
                    Execute(player, sent.Message[1..]);
                    return;
                }

                Broadcast(player.World, Message.Simple($"<{player.Username}> {sent.Message}"));
            });

            consumer.On<Tab>((_, tab) =>
            {
                tab.Matches = commands.Where(command => command.StartsWith(tab.Behind, StringComparison.OrdinalIgnoreCase)).ToArray();
            });

            // Keep the ground intact so nobody falls into the void.
            consumer.On<Dig>((_, dig) => dig.IsCancelled = dig.Position.Y < FlatGenerator.Height);
        });
    }

    private static readonly string[] commands = ["/help", "/spawn", "/list", "/give", "/clear"];

    private static void Execute(IPlayer player, string input)
    {
        var arguments = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = arguments.Length > 0 ? arguments[0] : string.Empty;

        switch (command.ToLowerInvariant())
        {
            case "help":
                player.Send(Message.Create().Write("Commands: ").Gray().Write(string.Join(", ", commands)).Build());
                break;

            case "spawn":
                player.Teleport(spawn);
                player.Send(Message.Create().Write("Teleported to spawn.").Green().Build());
                break;

            case "list":
                player.Send(Message.Simple($"Online ({player.World.Players.Count}): {string.Join(", ", player.World.Players.Keys)}"));
                break;

            case "give":
                Give(player, arguments[1..]);
                break;

            case "clear":
                player.Inventory.Clear();
                player.Send(Message.Create().Write("Cleared your inventory.").Green().Build());
                break;

            default:
                player.Send(Message.Create().Write($"Unknown command \"/{command}\".").Red().Build());
                break;
        }
    }

    private static void Give(IPlayer player, string[] arguments)
    {
        if (arguments.Length is 0 or > 2
            || !short.TryParse(arguments[0], out var type) || type <= 0
            || !byte.TryParse(arguments.ElementAtOrDefault(1) ?? "1", out var amount) || amount is 0 or > 64)
        {
            player.Send(Message.Create().Write("Usage: /give <id> [amount]").Red().Build());
            return;
        }

        // Put the item in the hand the player is holding.
        var inventory = player.Inventory;
        inventory.Set(Inventory.Hotbar + inventory.Selected, new Item(type, amount, 0));

        player.Send(Message.Create().Write($"Gave {amount} of {(ItemType) type}.").Green().Build());
    }

    private static void Broadcast(IWorld world, Message message)
    {
        foreach (var pair in world.Players)
        {
            pair.Value.Send(message);
        }
    }
}
