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
        foreach (var registered in registeredEvents.Where(registeredEvent => registeredEvent.Type == @event.GetType()))
        {
            var task = (Task?) registered.Delegate.DynamicInvoke(@event);

            if (task is not null)
            {
                await task;
            }
        }

        return @event;
    }
}