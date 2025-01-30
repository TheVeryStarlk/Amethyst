using Amethyst.Protocol;

namespace Amethyst.Components.Eventing.Sources.Client;

public sealed class Received(Message message) : Event<IClient>
{
    public Message Message { get; init; } = message;
}