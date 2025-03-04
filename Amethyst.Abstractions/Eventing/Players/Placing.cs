using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Eventing.Players;

public sealed class Placing(Position position, BlockFace face, Item item) : Event<IPlayer>
{
    public Position Position => position;

    public BlockFace Face => face;

    public Item Item => item;
}