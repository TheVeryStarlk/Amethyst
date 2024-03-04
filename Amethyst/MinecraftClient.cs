﻿using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftClient(ILogger<MinecraftClient> logger, ConnectionContext connection, int identifier) : IAsyncDisposable
{
    public int Identifier => identifier;

    private MinecraftClientState state;

    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (state is MinecraftClientState.Disconnected)
        {
            return;
        }

        logger.LogDebug("Stopping client");

        await source.CancelAsync();
        connection.Abort();

        state = MinecraftClientState.Disconnected;
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();
        await connection.DisposeAsync();
    }
}

internal enum MinecraftClientState
{
    Handshaking,
    Status,
    Login,
    Playing,
    Disconnected
}