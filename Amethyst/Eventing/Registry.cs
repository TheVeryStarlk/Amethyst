using System.Collections.Frozen;
using Amethyst.Components.Eventing;

namespace Amethyst.Eventing;

public sealed class Registry : IRegistry
{
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