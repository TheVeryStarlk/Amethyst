using Amethyst.Components;
using Amethyst.Components.Messages;

namespace Amethyst.Eventing.Sources.Servers;

public sealed class StatusRequest : Event<Server>
{
    public Status Status { get; set; } = Status.Create("Amethyst", 47, 0, 0, Message.Create("Hello, world!"), string.Empty);
}