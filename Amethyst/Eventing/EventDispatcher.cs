using System.Collections.Frozen;
using Amethyst.Components.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher(ILogger<EventDispatcher> logger, IEnumerable<ISubscriber> subscribers)
{
    private readonly FrozenDictionary<Type, IEnumerable<Delegate>> events = Registry.Create(subscribers);

    public TEvent Dispatch<TSource, TEvent>(TSource source, TEvent original)
    {
        if (!events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            return original;
        }

        // Maybe catch exceptions inside the loop?
        try
        {
            foreach (var task in callbacks.Cast<Action<TSource, TEvent>>())
            {
                task(source, original);
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