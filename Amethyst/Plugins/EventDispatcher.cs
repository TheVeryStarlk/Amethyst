using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal sealed class EventDispatcher : IEventDispatcher
{
    private readonly List<(Type Type, Delegate Delegate)> registeredEvents = [];

    public void Register<TEvent>(Func<TEvent, Task> @delegate) where TEvent : MinecraftEventBase
    {
        registeredEvents.Add((typeof(TEvent), @delegate));
    }

    public async Task<TEvent> DispatchAsync<TEvent>(TEvent @event) where TEvent : MinecraftEventBase
    {
        var registeredEvent = registeredEvents.FirstOrDefault(registeredEvent => registeredEvent.Type == @event.GetType());

        if (registeredEvent.Delegate is null)
        {
            return @event;
        }

        var task = (Task?) registeredEvent.Delegate.DynamicInvoke(@event);

        if (task is not null)
        {
            await task;
        }

        return @event;
    }
}