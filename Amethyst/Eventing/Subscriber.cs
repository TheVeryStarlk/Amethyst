using Amethyst.Components.Eventing;

namespace Amethyst.Eventing;

public sealed class Subscriber<TSource>(Dictionary<Type, Delegate> events) : ISubscriber<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>
    {
        if (!events.TryAdd(typeof(TEvent), callback))
        {
            throw new InvalidOperationException("Event is already registered.");
        }
    }
}