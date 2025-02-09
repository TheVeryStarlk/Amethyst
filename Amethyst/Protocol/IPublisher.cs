using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol;

internal interface IPublisher
{
    public Task PublishAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken);
}

internal sealed class DefaultPublisher(Packet packet) : IPublisher
{
    public async Task PublishAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(player, new Received(packet), cancellationToken).ConfigureAwait(false);
    }
}