using Amethyst.Abstractions.Eventing;

namespace Amethyst.Eventing;

internal sealed class Consumer<T>(Dictionary<Type, List<Delegate>> events) : IConsumer<T>
{
    public void On<TEvent>(Action<T, TEvent> callback) where TEvent : IEvent<T>
    {
        if (events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            callbacks.Add(callback);
            return;
        }

        events[typeof(TEvent)] = [callback];
    }
}
