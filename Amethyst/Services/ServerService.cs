using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Services;

internal sealed class ServerService(
    ILogger<ServerService> logger,
    Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await server.StartAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Ignore.
        }
        catch (Exception exception)
        {
            var bye = new[]
            {
                "I guess we got hit by a cosmic ray.",
                "Please don't forget to report this error. 😭",
                "At least I tried...",
                "Hopefully won't happen again...",
                "I got tired, bye..."
            };

            logger.LogCritical("Amethyst has crashed! {Message}",
                bye[Random.Shared.Next(bye.Length)]);

            logger.LogError(
                "Unexpected exception from server: \"{Exception}\"",
                exception);
        }
        finally
        {
            await server.StopAsync();
            await server.DisposeAsync();
        }
    }
}