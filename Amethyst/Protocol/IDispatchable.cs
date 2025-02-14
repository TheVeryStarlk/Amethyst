using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol;

internal interface IDispatchable
{
    public Task DispatchAsync(Player player, EventDispatcher eventDispatcher, CancellationToken cancellationToken);
}