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

    public void For<T>(Action<IListener<T>> configure)
    {
        var listener = new Listener<T>(events);
        configure(listener);
    }
}