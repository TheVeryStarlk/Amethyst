using System.Collections.Frozen;
using Microsoft.Extensions.Logging;

namespace Amethyst.Eventing;

internal sealed class EventDispatcher
{
    private readonly ILogger<EventDispatcher> logger;
    private readonly FrozenDictionary<Type, IEnumerable<Delegate>> events;

    public EventDispatcher(ILogger<EventDispatcher> logger, IEnumerable<ISubscriber> subscribers)
    {
        this.logger = logger;

        var dictionary = new Dictionary<Type, List<Delegate>>();
        var registry = new Registry(dictionary);

        foreach (var subscriber in subscribers)
        {
            subscriber.Subscribe(registry);
        }

        events = dictionary.ToFrozenDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());
    }

    public void Dispatch()
    {

    }
}