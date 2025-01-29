using Amethyst.Components.Eventing;

namespace Amethyst.Eventing;

public sealed class Subscriber<T>(Dictionary<Type, Delegate> events) : ISubscriber<T>
{
    public void On<TEvent>(Action<T, TEvent> callback) where TEvent : Event<T>
    {
        if (!events.TryAdd(typeof(TEvent), callback))
        {
            throw new InvalidOperationException("Event is already registered.");
        }
    }
}