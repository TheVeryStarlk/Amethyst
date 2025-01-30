using System.Collections.Frozen;
using Amethyst.Components.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher(ILogger<EventDispatcher> logger, ISubscriber subscriber)
{
    private readonly FrozenDictionary<Type, Delegate> events = Registry.Create(subscriber);

    public async Task<TEvent> DispatchAsync<TEvent, TSource>(TSource source, TEvent original, CancellationToken cancellationToken)
    {
        if (!events.TryGetValue(typeof(TEvent), out var value) || value is not TaskDelegate<TSource, TEvent> callback)
        {
            return original;
        }

        try
        {
            await callback(source, original, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception occurred while running event.");
        }

        return original;
    }
}