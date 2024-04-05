namespace Amethyst.Api.Plugins.Events;

public interface IEventService
{
    public Task<TEvent> ExecuteAsync<TEvent>(TEvent @event) where TEvent : MinecraftEventBase;
}