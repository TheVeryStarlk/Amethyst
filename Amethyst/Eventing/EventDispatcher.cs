using System.Collections.Frozen;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

public sealed class EventDispatcher(ILogger<EventDispatcher> logger, ISubscriber subscriber)
{
    private readonly FrozenDictionary<Type, Delegate> events = Registry.Create(subscriber);

    internal async Task<TEvent> DispatchAsync<TEvent, TSource>(TSource source, TEvent original, CancellationToken cancellationToken)
    {
        if (!events.TryGetValue(typeof(TEvent), out var value))
        {
            return original;
        }

        try
        {
            var callback = (TaskDelegate<TSource, TEvent>) value;
            await callback(source, original, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Nothing.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception occurred while running event.");
        }

        return original;
    }
}