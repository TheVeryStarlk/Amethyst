using Amethyst.Components.Eventing;

namespace Amethyst.Eventing;

public sealed class Listener<TSource>(Dictionary<Type, Delegate> events) : IListener<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>
    {
        if (!events.TryAdd(typeof(TEvent), callback))
        {
            throw new InvalidOperationException("Event is already registered.");
        }
    }
}