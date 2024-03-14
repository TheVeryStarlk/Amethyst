using Amethyst.Api.Events;

namespace Amethyst.Services;

internal sealed class EventService
{
    private readonly List<(Type Type, Delegate Delegate)> events = [];

    public void Register<TEventArgs>(Func<TEventArgs, Task> @delegate) where TEventArgs : AmethystEventArgsBase
    {
        events.Add((typeof(TEventArgs), @delegate));
    }

    public async Task<TEventArgs> ExecuteAsync<TEventArgs>(TEventArgs eventArgs) where TEventArgs : AmethystEventArgsBase
    {
        var @event = events.FirstOrDefault(@event => @event.Type == eventArgs.GetType());

        if (@event.Delegate is null)
        {
            return eventArgs;
        }

        var task = (Task?) @event.Delegate.DynamicInvoke(eventArgs);

        if (task is not null)
        {
            await task;
        }

        return eventArgs;
    }
}