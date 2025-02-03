using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Eventing.Sources.Client;

public sealed class StatusRequest : Event<IClient>
{
    public Status Status { get; set; } = Status.Create("Amethyst", 47, 0, 0, Message.Create("Hello, world!"), string.Empty);
}