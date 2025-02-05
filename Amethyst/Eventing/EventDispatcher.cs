using System.Collections.Frozen;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

public sealed class EventDispatcher
{
    private readonly ILogger<EventDispatcher> logger;
    private readonly FrozenDictionary<Type, ImmutableArray<Delegate>> events;

    public EventDispatcher(ILogger<EventDispatcher> logger, ISubscriber subscriber)
    {
        this.logger = logger;

        var registry = new Registry();

        // Register the internal subscriber first so that callbacks get to run before other subscribers.
        InternalSubscriber.Create(this, registry);
        subscriber.Subscribe(registry);

        events = registry.Build();
    }

    internal async Task<TEvent> DispatchAsync<TEvent, TSource>(TSource source, TEvent original, CancellationToken cancellationToken)
    {
        if (!events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            return original;
        }

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