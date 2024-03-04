using System.Net;

namespace Amethyst.Hosting;

public sealed class MinecraftServerConfiguration
{
    public IPEndPoint ListeningEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 25565);
}