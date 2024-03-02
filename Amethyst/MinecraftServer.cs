using System.Net;
using Amethyst.Components;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftServer(MinecraftServerConfiguration configuration) : IMinecraftServer
{
    private const int ProtocolVersion = 47;

    private readonly ILogger<MinecraftServer> logger = configuration.LoggerFactory.CreateLogger<MinecraftServer>();
    private readonly IPEndPoint listeningEndPoint = configuration.ListeningEndPoint;
    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public Status Status
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

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
    /// Stores the server's status, contains player information and MOTD.
    /// </summary>
    public Status Status { get; }

    /// <summary>
    /// Stops listening for connections & shutdowns the server's tasks.
    /// </summary>
    public void Shutdown();
}