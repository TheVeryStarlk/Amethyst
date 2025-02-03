using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Eventing.Sources.Server;

public sealed class Stopping : Event<IServer>
{
    public Message Message { get; set; } = "No reason specified.";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}