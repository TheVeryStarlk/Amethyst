using System.Net;

namespace Amethyst;

public sealed class MinecraftServerConfiguration
{
    public IPEndPoint ListeningEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 25565);
}