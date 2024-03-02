using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftClient(
    ILogger<MinecraftClient> logger,
    ConnectionContext connection) : IAsyncDisposable
{
    private MinecraftClientState state;

    public async ValueTask DisposeAsync()
    {
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