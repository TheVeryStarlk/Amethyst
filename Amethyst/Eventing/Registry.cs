using System.Collections.Frozen;
using Amethyst.Components.Eventing;

namespace Amethyst.Eventing;

public sealed class Registry : IRegistry
{
    private readonly Dictionary<Type, Delegate> events = [];

    public static FrozenDictionary<Type, Delegate> Create(Subscriber subscriber)
    {
        var registry = new Registry();

        subscriber.Subscribe(registry);

        return registry.events.ToFrozenDictionary();
    }

    public void For<T>(Action<ISubscriber<T>> configure)
    {
        var subscriber = new Subscriber<T>(events);
        configure(subscriber);
    }
}