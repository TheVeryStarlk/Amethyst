namespace Amethyst.Api.Plugins.Events.Server;

public sealed class ServerStartingEvent : MinecraftNotificationEventBase
{
    public required DateTimeOffset DateTimeOffset { get; init; }
}