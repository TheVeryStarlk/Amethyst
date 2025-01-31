using Amethyst.Components.Messages;

namespace Amethyst.Components.Eventing.Sources.Server;

public sealed class Stopping : Event<IServer>
{
    public Message Message { get; set; } = "No reason specified.";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}