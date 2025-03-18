using System.Net;
using Amethyst.Abstractions;

namespace Amethyst.Eventing.Server;

public sealed record Starting : Event<IServer>
{
    public IPEndPoint EndPoint { get; set; } = new(IPAddress.Any, 25565);
}