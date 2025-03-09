using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Players;

public sealed class Digging(DiggingStatus status, Position position, BlockFace face) : Event<IPlayer>
{
    public DiggingStatus Status => status;

    public Position Position => position;

    public BlockFace Face => face;
}

public enum DiggingStatus
{
    StartedDigging,
    CancelledDigging,
    FinishedDigging,
    DropItemStack,
    DropItem,
    ShootArrowOrFinishEating
}