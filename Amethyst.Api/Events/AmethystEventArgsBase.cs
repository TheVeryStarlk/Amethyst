namespace Amethyst.Api.Events;

public abstract class AmethystEventArgsBase
{
    public required IServer Server { get; init; }
}