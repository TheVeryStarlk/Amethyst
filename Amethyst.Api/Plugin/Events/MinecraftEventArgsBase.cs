namespace Amethyst.Api.Plugin.Events;

public abstract class MinecraftEventArgsBase
{
    public required IMinecraftServer Server { get; init; }
}