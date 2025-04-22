using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Server;

public sealed class Stopping : IEvent<IServer>
{
    public Message Message { get; set; } = Message.Simple("Bye!");
}