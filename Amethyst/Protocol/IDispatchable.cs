using Amethyst.Components.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol;

internal interface IDispatchable
{
    public Task DispatchAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken);
}