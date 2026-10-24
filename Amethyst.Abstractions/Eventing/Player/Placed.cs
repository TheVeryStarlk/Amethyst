using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Placed(Position position, Block block) : IEvent<IPlayer>, ICancellable
{
    public Position Position => position;

    public Block Block => block;

    public bool IsCancelled { get; set; }
}
