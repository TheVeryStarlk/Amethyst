using System.Collections.Concurrent;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Example.Time;
using Amethyst.Extensions;

namespace Amethyst.Example.Subscribers;

/// <summary>
/// Shows players where they are above their hotbar whenever they step onto another block.
/// </summary>
internal sealed class CompassSubscriber(Realm realm, Clock clock) : ISubscriber
{
    private readonly ConcurrentDictionary<IPlayer, (int X, int Y, int Z)> last = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Moved>((player, moved) =>
            {
                var current = (X: (int) Math.Floor(moved.Position.X), Y: (int) Math.Floor(moved.Position.Y), Z: (int) Math.Floor(moved.Position.Z));

                // Looking around also counts as moving.
                if (last.TryGetValue(player, out var previous) && previous == current)
                {
                    return;
                }

                last[player] = current;

                player.Send(Chat.Of(
                    $"{current.X} {current.Y} {current.Z}".Paint(Color.White),
                    "  |  ".Paint(Color.DarkGray),
                    realm.BiomeAt(moved.Position).Humanize().Paint(Color.Green),
                    "  |  ".Paint(Color.DarkGray),
                    clock.Display().Paint(Color.Aqua)), MessagePosition.HotBar);
            });

            consumer.On<Left>((player, _) => last.TryRemove(player, out var _));
        });
    }
}
