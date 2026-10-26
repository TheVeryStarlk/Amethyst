using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(ILogger<AmethystService> logger, IHostApplicationLifetime lifetime, Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await server.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unexpected exception occurred");

            // Exit with a failure so supervisors like Docker know the server crashed.
            Environment.ExitCode = 1;
        }
        finally
        {
            server.Dispose();
        }

        logger.LogInformation("Server stopped");

        // The host has nothing left to do without the server, for example when the port could not be bound.
        lifetime.StopApplication();
    }
}