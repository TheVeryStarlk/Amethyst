﻿using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api.Plugin.Events;

public sealed class PlayerLeaveEventArgs : MinecraftEventArgsBase
{
    public required IPlayer Player { get; set; }

    public required ChatMessage Message { get; set; }
}