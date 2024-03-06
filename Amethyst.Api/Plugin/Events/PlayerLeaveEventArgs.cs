using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api.Plugin.Events;

public sealed class PlayerLeaveEventArgs : MinecraftEventArgsBase
{
    public required IPlayer Player { get; init; }

    public required ChatMessage Message { get; init; }
}