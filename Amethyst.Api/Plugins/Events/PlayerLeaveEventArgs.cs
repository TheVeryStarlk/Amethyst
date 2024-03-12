using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api.Plugins.Events;

public sealed class PlayerLeaveEventArgs : ServerEventArgsBase
{
    public required IPlayer Player { get; set; }

    public required ChatMessage Message { get; set; }
}