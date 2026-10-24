using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Dig(Digging status, Position position, BlockFace face) : IEvent<IPlayer>
{
    public Digging Status => status;

    public Position Position => position;

    public BlockFace Face => face;
}