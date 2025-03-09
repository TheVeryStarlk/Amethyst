using System.Collections.Frozen;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher
{
    private readonly ILogger<EventDispatcher> logger;
    private readonly FrozenDictionary<Type, IEnumerable<Delegate>> events;

    public EventDispatcher(ILogger<EventDispatcher> logger, IEnumerable<ISubscriber> subscribers)
    {
        this.logger = logger;

        var dictionary = new Dictionary<Type, List<Delegate>>();
        var registry = new Registry(dictionary);

        foreach (var subscriber in subscribers)
        {
            subscriber.Subscribe(registry);
        }

        events = dictionary.ToFrozenDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());
    }

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