using Microsoft.Extensions.Logging;

namespace Amethyst.Commands;

internal sealed class CommandService(ILogger<CommandService> logger) : IAsyncDisposable
{
    public void Load()
    {
        logger.LogInformation("Loading commands");
    }

    public Task ExecuteCommandAsync()
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}