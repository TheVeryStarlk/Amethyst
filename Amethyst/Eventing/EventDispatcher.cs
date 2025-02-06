using System.Collections.Frozen;
using Amethyst.Abstractions.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher(ILogger<EventDispatcher> logger, IEnumerable<ISubscriber> subscribers)
{
    private readonly FrozenDictionary<Type, IEnumerable<Delegate>> events = Registry.Create(subscribers);

    public async Task<TEvent> DispatchAsync<TEvent, TSource>(TSource source, TEvent original, CancellationToken cancellationToken)
    {
        if (!events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            return original;
        }

        // Maybe catch exceptions inside the loop?
        try
        {
            foreach (var task in callbacks.Cast<TaskDelegate<TSource, TEvent>>())
            {
                await task(source, original, cancellationToken).ConfigureAwait(false);
            }
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