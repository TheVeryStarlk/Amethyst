using Amethyst.Api.Events;

namespace Amethyst.Plugins;

internal sealed class EventService : IEventService
{
    public Task<TEvent> ExecuteAsync<TEvent>(TEvent @event) where TEvent : MinecraftEventBase
    {
        throw new NotImplementedException();
    }
}

internal interface IEventService
{
    public Task<TEvent> ExecuteAsync<TEvent>(TEvent @event) where TEvent : MinecraftEventBase;
}