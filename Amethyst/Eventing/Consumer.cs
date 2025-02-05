namespace Amethyst.Eventing;

public sealed class Consumer<TSource>(Dictionary<Type, List<Delegate>> events) : IConsumer<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>
    {
        if (events.TryGetValue(typeof(TEvent), out var callbacks))
        {
            callbacks.Add(callback);
            return;
        }

        events[typeof(TEvent)] = [callback];
    }
}