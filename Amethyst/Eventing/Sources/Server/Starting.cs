using System.Net;
using Amethyst.Abstractions;

namespace Amethyst.Eventing.Sources.Server;

public sealed class Starting : Event<IServer>
{
    public IPEndPoint EndPoint { get; set; } = new(IPAddress.Any, 25565);
}