using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(Server server, ILogger<AmethystService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            server.Start();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred.");
        }
        finally
        {
            server.Dispose();
        }

        return Task.CompletedTask;
    }
}