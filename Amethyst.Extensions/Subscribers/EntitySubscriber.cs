using System.Collections.Concurrent;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Shows the entities of a world to players joining it, players are not entities of a world.
/// </summary>
public sealed class EntitySubscriber : ISubscriber
{
    // The world each player was last shown the entities of.
    private readonly ConcurrentDictionary<IPlayer, IWorld> worlds = [];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Moved>((source, _) =>
            {
                // Clients move right after joining or changing worlds, and clear their entities when changing worlds.
                if (worlds.TryGetValue(source, out var world) && world == source.World)
                {
                    return;
                }

                worlds[source] = source.World;

                foreach (var pair in source.World.Entities)
                {
                    pair.Value.Show(source);
                }
            });

            consumer.On<Left>((source, _) => worlds.TryRemove(source, out var world));
        });
    }
}