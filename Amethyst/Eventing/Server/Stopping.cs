using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Server;

public sealed class Stopping : Event<IServer>
{
    public Message Message { get; set; } = Message.Simple("Bye!");
}