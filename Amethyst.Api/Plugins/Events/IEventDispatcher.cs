namespace Amethyst.Api.Plugins.Events;

public interface IEventDispatcher
{
    public Task<TEvent> DispatchAsync<TEvent>(TEvent @event) where TEvent : MinecraftEventBase;
}