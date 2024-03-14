using Amethyst.Api.Commands;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Microsoft.Extensions.Logging;

namespace Amethyst.Services;

internal sealed class CommandService(ILogger<CommandService> logger)
{
    private readonly Dictionary<string, Func<Command, Task>> commands = [];

    public void Register(string name, Func<Command, Task> @delegate)
    {
        if (!commands.TryAdd(name, @delegate))
        {
            logger.LogWarning("Could not register command: \"{Name}\"", name);
        }
    }

    public async Task ExecuteAsync(IPlayer player, string message)
    {
        var split = message.Split(" ");

        if (split[0].Length < 1)
        {
            await player.SendChatMessageAsync(ChatMessage.Create("Invalid command.", Color.Red));
            return;
        }

        var name = split[0];

        var predicate = commands
            .Where(command => command.Key == name)
            .ToArray();

        if (predicate.Length == 0)
        {
            await player.SendChatMessageAsync(ChatMessage.Create(
                $"Could not find command: \"{split[0]}\".",
                Color.Red));

            return;
        }

        foreach (var command in predicate)
        {
            await (Task) command.Value.DynamicInvoke(new Command
            {
                Server = player.Server,
                Player = player,
                Arguments = split[1..]
            })!;
        }

        logger.LogInformation("Executed command {Name}", name);
    }
}