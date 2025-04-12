using System.Collections.Frozen;

namespace Amethyst.Hosting.Subscribing;

public interface IRegistry
{
    public void For<T>(Action<IConsumer<T>> configure);
}

internal sealed class Registry(Dictionary<Type, List<Delegate>> events) : IRegistry
{
    public static FrozenDictionary<Type, FrozenSet<Delegate>> Create(IEnumerable<ISubscriber> subscribers)
    {
        var dictionary = new Dictionary<Type, List<Delegate>>();
        var registry = new Registry(dictionary);

        foreach (var subscriber in subscribers)
        {
            subscriber.Subscribe(registry);
        }

        return dictionary.ToFrozenDictionary(pair => pair.Key, pair => pair.Value.ToFrozenSet());
    }

    public void For<T>(Action<IConsumer<T>> configure)
    {
        var consumer = new Consumer<T>(events);
        configure(consumer);
    }
}