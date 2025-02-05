using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Amethyst.Eventing;

public sealed class Registry : IRegistry
{
    private readonly Dictionary<Type, List<Delegate>> events = [];

    public void For<T>(Action<IConsumer<T>> configure)
    {
        var consumer = new Consumer<T>(events);
        configure(consumer);
    }

    internal FrozenDictionary<Type, ImmutableArray<Delegate>> Build()
    {
        return events.ToFrozenDictionary(pair => pair.Key, pair => pair.Value.ToImmutableArray());
    }
}