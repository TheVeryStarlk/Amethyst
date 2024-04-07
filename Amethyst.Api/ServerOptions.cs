using System.Net;
using Amethyst.Api.Components;

namespace Amethyst.Api;

public sealed class ServerOptions
{
    public IPEndPoint EndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 25565);

    public TimeSpan IdleTimeOut { get; set; } = TimeSpan.FromSeconds(5);

    public Chat Description { get; set; } = "Hello, world!";

    public int MaximumPlayers { get; set; } = 20;

    public byte ViewDistance { get; set; } = 2;
}