using Amethyst.Api.Components;

namespace Amethyst.Api.Events.Minecraft.Player;

public sealed class PlayerJoinedEventArgs : PlayerEventArgsBase
{
    public required ChatMessage Message { get; set; }

    public DateTimeOffset DateTimeOffset { get; init; }
}