﻿using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class CommandService(ILogger<CommandService> logger)
{
    public HashSet<CommandWrapper> Commands { get; } = [];

    public async Task ExecuteCommandAsync(IPlayer player, string message)
    {
        var split = message.Split(" ");

        if (split[0].Length < 1)
        {
            await player.SendChatMessageAsync(ChatMessage.Create("Invalid command.", Color.Red));
            return;
        }

        var name = split[0];

        var predicate = Commands
            .Where(command => command.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();

        if (predicate.Length == 0)
        {
            await player.SendChatMessageAsync(
                ChatMessage.Create($"Could not find command: \"{split[0]}\".", Color.Red));
            return;
        }

        logger.LogInformation("Executing command {Name}", name);

        foreach (var command in predicate)
        {
            await (Task) command.Delegate.DynamicInvoke(new CommandContext
            {
                Player = player,
                Name = split[0],
                Arguments = split[1..]
            })!;
        }
    }
}

internal sealed record CommandWrapper(string Name, Delegate Delegate);