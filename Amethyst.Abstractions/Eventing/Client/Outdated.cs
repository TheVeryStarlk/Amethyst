using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Eventing.Client;

public sealed class Outdated : IEvent<IClient>
{
    public Message Message { get; set; } = Message.Simple("Outdated!");
}