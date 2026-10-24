using System.Net;
using Amethyst.Abstractions;

namespace Amethyst.Abstractions.Eventing.Server;

public sealed class Starting : IEvent<IServer>
{
    public IPEndPoint EndPoint { get; set; } = new(IPAddress.Any, 25565);
}