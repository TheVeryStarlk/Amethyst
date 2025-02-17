namespace Amethyst.Components.Eventing;

public interface IConsumer<TSource>
{
    public void On<TEvent>(Action<TSource, TEvent> callback) where TEvent : Event<TSource>;
}