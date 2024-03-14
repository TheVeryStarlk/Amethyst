using Amethyst.Api.Components;

namespace Amethyst.Api.Events.Minecraft;

public sealed class DescriptionRequestedEventArgs : AmethystEventArgsBase
{
    public required ChatMessage Description { get; set; }
}