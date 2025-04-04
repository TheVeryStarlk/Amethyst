using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(ILogger<AmethystService> logger, Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await server.StartAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unexpected exception occurred");
        }
        finally
        {
            server.Dispose();
        }

        logger.LogInformation("Server stopped");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        server.Stop();
        return base.StopAsync(cancellationToken);
    }
}