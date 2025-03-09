using Amethyst.Abstractions.Eventing;

namespace Amethyst.Eventing;

public sealed class Consumer<TSource>(Dictionary<Type, List<Delegate>> events)
{
    public void On<TEvent>(Action<TSource, TEvent> callback) where TEvent : Event<TSource>
    {
        if (events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            callbacks.Add(callback);
            return;
        }

        events[typeof(TEvent)] = [callback];
    }
}