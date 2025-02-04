using System.Net;

namespace Amethyst.Eventing.Sources.Servers;

public sealed class Starting : Event<Server>
{
    public IPEndPoint EndPoint { get; set; } = new(IPAddress.Any, 25565);
}