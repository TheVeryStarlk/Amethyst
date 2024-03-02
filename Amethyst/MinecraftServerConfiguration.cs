using System.Net;
using Amethyst.Components;
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
    public required IPEndPoint ListeningEndPoint { get; set; }

    /// <summary>
    /// Sets the message of the day of the <see cref="MinecraftServer"/> as known as MOTD.
    /// </summary>
    public required Chat MessageOfTheDay { get; set; }

    /// <summary>
    /// Sets the maximum amount of players that can join the <see cref="MinecraftServer"/>.
    /// </summary>
    public required int MaximumPlayerCount { get; set; }

    /// <summary>
    /// The logger factory which <see cref="MinecraftServer"/> uses to create loggers.
    /// </summary>
    public required ILoggerFactory LoggerFactory { get; set; }
}