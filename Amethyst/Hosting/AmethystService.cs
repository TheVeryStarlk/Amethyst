using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(ILogger<AmethystService> logger, Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await server.StartAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected exception");
        }
        finally
        {
            server.Dispose();
        }
    }
}