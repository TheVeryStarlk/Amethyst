namespace Amethyst.Abstractions.Eventing;

public interface IConsumer<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>;
}