namespace Amethyst.Api.Plugins.Events;

public abstract class ServerEventArgsBase
{
    public required IServer Server { get; init; }
}