using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Client;

public sealed class Outdated : Event<IClient>
{
    public Message Message { get; set; } = Message.Simple("Outdated!");
}