namespace Amethyst.Components.Eventing;

public interface IConsumer<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>;
}