using Amethyst.Api.Components;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Api.Events.Minecraft.Player;

public sealed class BlockBreakEventArgs : PlayerEventArgsBase
{
    public required Block Block { get; set; }

    public required Position Position { get; set; }
}