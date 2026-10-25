namespace Amethyst.Abstractions.Eventing;

public interface IConsumer<T>
{
    public void On<TEvent>(Action<T, TEvent> callback) where TEvent : IEvent<T>;
}