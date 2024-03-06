using Amethyst.Api.Plugins.Events;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class EventService(ILogger<EventService> logger)
{
    public HashSet<EventWrapper> Events { get; } = [];

    public async Task ExecuteEventAsync<T>(T eventArgs) where T : MinecraftEventArgsBase
    {
        logger.LogDebug("Executing event: \"{Type}\"", typeof(T).FullName);

        foreach (var @event in Events.Where(@event => @event.EventArgs == typeof(T)))
        {
            await (Task) @event.Delegate.DynamicInvoke(eventArgs)!;
        }
    }
}

internal sealed record EventWrapper(Type EventArgs, Delegate Delegate);