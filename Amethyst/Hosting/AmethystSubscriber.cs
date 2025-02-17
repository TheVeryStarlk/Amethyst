using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Entities;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber(IPlayerStore store) : ISubscriber
{
    private readonly PlayerStore playerStore = (PlayerStore) store;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((source, _) =>
            {
                if (!playerStore.TryAdd(source))
                {
                    source.Disconnect("Player with same username already exists.");
                }
            });

            consumer.On<Left>((source, _) => playerStore.Remove(source));
            consumer.On<Moved>((source, _) => ((Player) source).Update());
        });
    }
}