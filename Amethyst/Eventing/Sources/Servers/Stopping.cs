using Amethyst.Components.Messages;

namespace Amethyst.Eventing.Sources.Servers;

public sealed class Stopping : Event<Server>
{
    public Message Message { get; set; } = "No reason specified.";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}