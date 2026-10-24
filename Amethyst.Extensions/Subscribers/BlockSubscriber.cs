using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Applies the blocks players break and place to their world, and shows them to the other players.
/// </summary>
public sealed class BlockSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Dig>((source, dig) =>
            {
                // Creative clients break blocks instantly, so they never send a finished status.
                var broken = dig.Status is Digging.Finished || source.GameMode is GameMode.Creative && dig.Status is Digging.Started;

                if (broken)
                {
                    source.World.Set(dig.Position, Blocks.Air, source);
                }
            });

            consumer.On<Placed>((source, placed) => source.World.Set(placed.Position, placed.Block, source));
        });
    }
}
