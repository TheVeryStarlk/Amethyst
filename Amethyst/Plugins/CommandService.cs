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
        await player.SendChatAsync(chat);
    }
}