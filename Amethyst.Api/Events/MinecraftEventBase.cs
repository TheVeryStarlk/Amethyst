namespace Amethyst.Api.Events;

public abstract class MinecraftEventBase
{
    public required IServer Server { get; init; }

    public bool IsCancelled { get; set; }
}