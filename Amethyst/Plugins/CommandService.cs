using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins.Commands;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class CommandService(ILogger<CommandService> logger) : ICommandService
{
    public IEnumerable<string> Commands => registeredCommands.Keys;

    private readonly Dictionary<string, Func<CommandInformation, Task>> registeredCommands = [];

    public void Register(string name, Func<CommandInformation, Task> @delegate)
    {
        _ = registeredCommands.TryAdd(name, @delegate);
    }

    public async Task ExecuteAsync(IPlayer player, string chat)
    {
        if (chat.Length == 1 || chat.Length > 1 && char.IsWhiteSpace(chat[1]))
        {
            var error = Chat.Create("Invalid command syntax.", Color.Red);
            await player.SendChatAsync(error);
            return;
        }

        var parts = chat.Split(' ');
        var name = parts[0][1..].ToLower();

        if (registeredCommands.All(command => command.Key != name))
        {
            var error = Chat.Create("Unknown command.", Color.Red);
            await player.SendChatAsync(error);
            return;
        }

        var command = registeredCommands.First(command => command.Key == name).Value;

        await command(
            new CommandInformation
            {
                Server = player.Server,
                Player = player,
                Arguments = parts.Length == 1
                    ? Array.Empty<string>()
                    : [chat[(parts[0].Length + 1)..]]
            });
    }
}