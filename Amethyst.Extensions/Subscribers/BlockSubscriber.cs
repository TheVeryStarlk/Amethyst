using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Packets.Play;
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

            consumer.On<Placed>((source, placed) =>
            {
                source.World.Set(placed.Position, placed.Block, source);

                // Clients do not play the sound of blocks they place, so the source hears it from the server too.
                // Every block sounds like stone, played from its center a bit lower like vanilla does.
                var center = new Position(placed.Position.X + 0.5D, placed.Position.Y + 0.5D, placed.Position.Z + 0.5D);
                var packet = new SoundPacket(Sounds.DigStone, center, 1F, 0.8F);

                foreach (var pair in source.World.Players)
                {
                    pair.Value.Client.Write(packet);
                }
            });
        });
    }
}