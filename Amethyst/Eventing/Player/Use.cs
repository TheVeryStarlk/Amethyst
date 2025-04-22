using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Use(Position position, Item item) : IEvent<IPlayer>
{
    public Position Position => position;

    public Item Item => item;
}