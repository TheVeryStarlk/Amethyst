using Amethyst.Components.Messages;

namespace Amethyst.Components.Eventing.Sources.Servers;

public sealed class Stopping : Event<Stopping>
{
    public Message Message { get; set; } = "No reason specified.";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}