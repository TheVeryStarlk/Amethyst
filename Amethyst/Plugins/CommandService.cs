using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class CommandService(ILogger<CommandService> logger)
{
    public HashSet<CommandWrapper> Commands { get; } = [];

    public async Task ExecuteCommandAsync()
    {

    }
}

internal sealed record CommandWrapper(string Name, Delegate Delegate);