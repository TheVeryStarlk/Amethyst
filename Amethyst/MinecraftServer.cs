using System.Net;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftServer(MinecraftServerConfiguration configuration) : IMinecraftServer
{
    private readonly ILogger<MinecraftServer> logger = configuration.LoggerFactory.CreateLogger<MinecraftServer>();
    private readonly IPEndPoint listeningEndPoint = configuration.ListeningEndPoint;
    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public void Shutdown()
    {
        logger.LogDebug("Shutting down the server");
        source.Cancel();
    }
}

/// <summary>
/// Represents a Minecraft server.
/// </summary>
internal interface IMinecraftServer
{
    /// <summary>
    /// Stops listening for connections & shutdowns the server's tasks.
    /// </summary>
    public void Shutdown();
}