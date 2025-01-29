using System.Collections.Frozen;

namespace Amethyst.Hosting.Subscriber;

public interface IRegistry
{
    public void For<T>(Action<ISubscriber<T>> configure);
}

public sealed class Registry : IRegistry
{
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

    private readonly Dictionary<Type, Delegate> events = [];

    public void For<T>(Action<ISubscriber<T>> configure)
    {
        var subscriber = new Subscriber<T>(events);
        configure(subscriber);
    }

    public FrozenDictionary<Type, Delegate> Build()
    {
        return events.ToFrozenDictionary();
    }
}