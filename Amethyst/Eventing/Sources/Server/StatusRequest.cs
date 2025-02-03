using Amethyst.Abstractions;
using Amethyst.Components;
using Amethyst.Components.Messages;

namespace Amethyst.Eventing.Sources.Server;

public sealed class StatusRequest : Event<IServer>
{
    public Status Status { get; set; } = Status.Create("Amethyst", 47, 0, 0, Message.Create("Hello, world!"), string.Empty);
}