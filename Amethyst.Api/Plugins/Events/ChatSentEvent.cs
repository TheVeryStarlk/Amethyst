using Amethyst.Api.Components;

namespace Amethyst.Api.Plugins.Events;

public sealed class ChatSentEvent : MinecraftHandlerEventBase
{
    public required Chat Chat { get; set; }
}