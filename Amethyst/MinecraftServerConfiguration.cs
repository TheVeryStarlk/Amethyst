using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Amethyst;

internal sealed class MinecraftServerConfiguration
{
    public IPEndPoint ListeningEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 25565);

    public int MaximumPlayerCount { get; set; } = 20;

    public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;
}