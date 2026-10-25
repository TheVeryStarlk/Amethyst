using System.Collections.Frozen;
using Amethyst.Abstractions.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher(ILogger<EventDispatcher> logger, IEnumerable<ISubscriber> subscribers)
{
    private readonly FrozenDictionary<Type, Delegate[]> events = Registry.Create(subscribers);

    public TEvent Dispatch<TSource, TEvent>(TSource source, TEvent original) where TEvent : IEvent<TSource>
    {
        if (!events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            return original;
        }

        // Callbacks are registered as Action<TAbstraction, TEvent>, which is contravariant in the source.
        foreach (var callback in callbacks)
        {
            try
            {
                ((Action<TSource, TEvent>) callback)(source, original);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An exception occurred while running event: '{Name}'", typeof(TEvent).Name);
            }

            if (original is ICancellable { IsCancelled: true })
            {
                break;
            }
        }

        return original;
    }
}