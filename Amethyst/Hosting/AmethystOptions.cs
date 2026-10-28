using System.Net;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Hosting;

public sealed class AmethystOptions
{
    public IPEndPoint EndPoint { get; set; } = new(IPAddress.Any, 25565);

    public Message Message { get; set; } = Message.Simple("Bye!");
}