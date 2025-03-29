using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(ILogger<AmethystService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hello, world!");
        return Task.CompletedTask;
    }
}