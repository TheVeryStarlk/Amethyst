using System.Net;

namespace Amethyst.Api;

public sealed class ServerOptions
{
    public IPEndPoint EndPoint { get; init; } = new IPEndPoint(IPAddress.Any, 25565);

    public TimeSpan IdleTimeOut { get; init; } = TimeSpan.FromSeconds(5);
}