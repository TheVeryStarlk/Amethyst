using Amethyst.Api.Components;

namespace Amethyst.Api.Plugin.Events;

public sealed class DescriptionRequestedEventArgs : MinecraftEventArgsBase
{
    public required ChatMessage Description { get; set; }
}