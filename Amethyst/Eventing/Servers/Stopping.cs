using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Servers;

public sealed class Stopping : Event<Stopping>
{
    public Message Message { get; set; } = "No reason specified.";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}