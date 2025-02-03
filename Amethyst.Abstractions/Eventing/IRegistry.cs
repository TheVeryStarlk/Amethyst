namespace Amethyst.Abstractions.Eventing;

public interface IRegistry
{
    public void For<T>(Action<IConsumer<T>> configure);
}

public interface IConsumer<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>;
}