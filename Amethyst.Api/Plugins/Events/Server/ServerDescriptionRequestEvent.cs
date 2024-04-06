using Amethyst.Api.Components;

namespace Amethyst.Api.Plugins.Events.Server;

public sealed class ServerDescriptionRequestEvent : MinecraftNotificationEventBase
{
    public required Chat Description { get; set; }
}