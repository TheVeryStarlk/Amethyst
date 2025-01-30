using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class AmethystService(ILogger<AmethystService> logger, Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        try
        {
            await server.StartAsync(cancellationToken).ConfigureAwait(false);
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