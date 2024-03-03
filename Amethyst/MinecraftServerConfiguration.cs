using System.Net;
using Amethyst.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Amethyst;

/// <summary>
/// Stores configuration values to control <see cref="IMinecraftServer"/>.
/// </summary>
internal sealed class MinecraftServerConfiguration
{
    /// <summary>
    /// The remote <see cref="IPEndPoint"/> the <see cref="IMinecraftServer"/> should listen to.
    /// </summary>
    public required IPEndPoint ListeningEndPoint { get; set; }

    /// <summary>
    /// Sets the message of the day (MOTD) of the <see cref="IMinecraftServer"/>.
    /// </summary>
    public required Chat MessageOfTheDay { get; set; }

    /// <summary>
    /// Sets the maximum amount of players that can join the <see cref="IMinecraftServer"/>.
    /// </summary>
    public required int MaximumPlayerCount { get; set; }

    /// <summary>
    /// The logger factory which <see cref="IMinecraftServer"/> uses to create loggers.
    /// </summary>
    /// <remarks>
    /// By default uses <see cref="NullLoggerFactory"/>.
    /// </remarks>
    public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;
}