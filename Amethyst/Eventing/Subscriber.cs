using Amethyst.Abstractions.Eventing;

namespace Amethyst.Eventing;

public sealed class Consumer<TSource>(Dictionary<Type, Delegate> events) : IConsumer<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>
    {
        if (!events.TryAdd(typeof(TEvent), callback))
        {
            throw new InvalidOperationException("Event is already registered.");
        }
    }
}