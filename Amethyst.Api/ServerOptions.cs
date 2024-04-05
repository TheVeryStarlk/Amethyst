using System.Net;
using Amethyst.Api.Components;

namespace Amethyst.Api;

public sealed class ServerOptions
{
    public IPEndPoint EndPoint { get; init; } = new IPEndPoint(IPAddress.Any, 25565);

    public TimeSpan IdleTimeOut { get; init; } = TimeSpan.FromSeconds(5);

    public Chat Description { get; init; } = "Hello, world!";

    public int MaximumPlayers { get; init; }
}