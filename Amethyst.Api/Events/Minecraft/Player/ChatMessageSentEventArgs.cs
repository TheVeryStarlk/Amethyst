using Amethyst.Api.Components;

namespace Amethyst.Api.Events.Minecraft.Player;

public sealed class ChatMessageSentEventArgs : PlayerEventArgsBase
{
    public required ChatMessage Message { get; set; }
}