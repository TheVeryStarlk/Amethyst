using Amethyst.Api.Components;

namespace Amethyst.Api.Plugins.Events;

public sealed class DescriptionRequestedEventArgs : MinecraftEventArgsBase
{
    public required ChatMessage Description { get; set; }
}