using System.Net;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftServerConfiguration
{
    public required IPEndPoint ListeningEndPoint { get; init; }

    public required ILoggerFactory LoggerFactory { get; init; }
}