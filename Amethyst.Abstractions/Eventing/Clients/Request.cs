using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Eventing.Clients;

public sealed class Request : Event<IClient>
{
    public Status Status { get; set; } = Status.Create("Amethyst", 47, 0, 0, Message.Create("Hello, world!"), string.Empty);
}