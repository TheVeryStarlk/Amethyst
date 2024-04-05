namespace Amethyst.Api.Plugins.Events.Server;

public sealed class ServerStoppingEvent : MinecraftNotificationEventBase
{
    public required DateTimeOffset DateTimeOffset { get; init; }
}