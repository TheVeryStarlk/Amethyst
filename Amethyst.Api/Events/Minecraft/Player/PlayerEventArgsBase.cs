using Amethyst.Api.Entities;

namespace Amethyst.Api.Events.Minecraft.Player;

public abstract class PlayerEventArgsBase : AmethystEventArgsBase
{
    public required IPlayer Player { get; init; }
}