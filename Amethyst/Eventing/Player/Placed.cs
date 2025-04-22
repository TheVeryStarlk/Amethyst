using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Player;

public sealed class Placed(Position position, Block block) : IEvent<IPlayer>
{
    public Position Position => position;

    public Block Block => block;
}