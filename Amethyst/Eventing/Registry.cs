using System.Collections.Frozen;
using Amethyst.Abstractions.Eventing;

namespace Amethyst.Eventing;

internal sealed class Registry(Dictionary<Type, List<Delegate>> events) : IRegistry
{
    // Callbacks are kept in registration order.
    public static FrozenDictionary<Type, Delegate[]> Create(IEnumerable<ISubscriber> subscribers)
    {
        var dictionary = new Dictionary<Type, List<Delegate>>();
        var registry = new Registry(dictionary);

        foreach (var subscriber in subscribers)
        {
            subscriber.Subscribe(registry);
        }

        return dictionary.ToFrozenDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }

    public void For<T>(Action<IConsumer<T>> configure)
    {
        var consumer = new Consumer<T>(events);
        configure(consumer);
    }
}