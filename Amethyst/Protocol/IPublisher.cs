using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol;

internal interface IPublisher
{
    public Task PublishAsync(Packet packet, IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken);
}