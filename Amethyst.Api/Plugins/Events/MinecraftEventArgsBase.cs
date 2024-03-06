namespace Amethyst.Api.Plugins.Events;

public abstract class MinecraftEventArgsBase
{
    public required IMinecraftServer Server { get; init; }
}