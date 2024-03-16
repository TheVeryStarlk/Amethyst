using Amethyst.Levels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Services;

internal sealed class LevelService(ILogger<LevelService> logger, Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var level = (Level) server.Level!;

        while (cancellationToken.IsCancellationRequested)
        {
            try
            {
                await level.TickAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception from server: \"{Exception}\"",
                    exception);

                break;
            }
            finally
            {
                await level.DisposeAsync();
            }
        }
    }
}