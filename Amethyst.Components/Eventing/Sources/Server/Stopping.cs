namespace Amethyst.Components.Eventing.Sources.Server;

public sealed class Stopping : Event<IServer>
{
    public string Message { get; set; } = "No reason specified.";

    public TimeSpan Timeout { get; set; }
}