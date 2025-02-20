using System.Collections.Frozen;
using Amethyst.Abstractions.Eventing;

namespace Amethyst.Eventing;

internal sealed class Registry(Dictionary<Type, List<Delegate>> events) : IRegistry
{
    public static FrozenDictionary<Type, IEnumerable<Delegate>> Create(IEnumerable<ISubscriber> subscribers)
    {
        var events = new Dictionary<Type, List<Delegate>>();
        var registry = new Registry(events);

        foreach (var subscriber in subscribers)
        {
            subscriber.Subscribe(registry);
        }

        return events.ToFrozenDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());
    }

    public void For<T>(Action<IConsumer<T>> configure)
    {
        var consumer = new Consumer<T>(events);
        configure(consumer);
    }
}