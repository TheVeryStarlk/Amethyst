using System.Net;
using Microsoft.Extensions.Logging;

namespace Amethyst;

/// <summary>
/// Stores configuration values to control <see cref="MinecraftServer"/>.
/// </summary>
internal sealed class MinecraftServerConfiguration
{
    /// <summary>
    /// The remote <see cref="IPEndPoint"/> the <see cref="MinecraftServer"/> should listen to.
    /// </summary>
    public required IPEndPoint ListeningEndPoint { get; init; }

    /// <summary>
    /// The logger factory which <see cref="MinecraftServer"/> uses to create loggers.
    /// </summary>
    public required ILoggerFactory LoggerFactory { get; init; }
}