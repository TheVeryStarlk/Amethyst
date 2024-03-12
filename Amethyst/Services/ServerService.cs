﻿using Microsoft.Extensions.Hosting;
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
            await server.StartAsync();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(
                "Unexpected exception from server: \"{Message}\"",
                exception);
        }
        finally
        {
            await server.StopAsync();
            await server.DisposeAsync();
        }
    }
}