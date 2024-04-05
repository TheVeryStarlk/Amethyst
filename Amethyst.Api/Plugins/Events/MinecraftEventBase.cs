namespace Amethyst.Api.Plugins.Events;

public abstract class MinecraftEventBase
{
    public required IServer Server { get; init; }

}

public abstract class MinecraftHandlerEventBase : MinecraftEventBase
{
    public bool IsHandled { get; set; }
}

public abstract class MinecraftNotificationEventBase : MinecraftEventBase;