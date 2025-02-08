using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(Server server, ILogger<AmethystService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting server");

        try
        {
            await server.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred.");
        }
        finally
        {
            server.Dispose();
        }

        logger.LogInformation("Server stopped");
    }
}