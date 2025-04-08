using System.Collections.Frozen;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher(ILogger<EventDispatcher> logger, IEnumerable<ISubscriber> subscribers)
{
    private readonly FrozenDictionary<Type, FrozenSet<Delegate>> events = Registry.Create(subscribers);

    // Could use some better constraints.
    public TEvent Dispatch<TSource, TEvent>(TSource source, TEvent original)
    {
        if (!events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            return original;
        }

        // Maybe catch exceptions inside the loop?
        try
        {
            foreach (var callback in callbacks.Cast<Action<TSource, TEvent>>())
            {
                callback(source, original);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception occurred while running event: '{Name}'", typeof(TEvent).Name);
        }

        return original;
    }
}