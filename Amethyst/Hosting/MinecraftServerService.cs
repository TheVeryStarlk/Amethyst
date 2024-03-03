﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class MinecraftServerService(ILogger<MinecraftServerService> logger, MinecraftServer server) : BackgroundService
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
                exception.Message);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await server.StopAsync();
        await base.StopAsync(CancellationToken.None);
    }
}