using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Api.Events.Minecraft.Player;

public sealed class BlockPlaceEventArgs : PlayerEventArgsBase
{
    public required Block Block { get; set; }

    public required Position Position { get; set; }

    public required string Sound { get; set; }
}