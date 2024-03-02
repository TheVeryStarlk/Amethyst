using System.Net;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftServer(MinecraftServerConfiguration configuration) : IMinecraftServer
{
    private readonly ILogger<MinecraftServer> logger = configuration.LoggerFactory.CreateLogger<MinecraftServer>();
    private readonly IPEndPoint endPoint = configuration.ListeningEndPoint;
}

internal interface IMinecraftServer;