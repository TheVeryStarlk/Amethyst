namespace Amethyst.Components.Eventing;

public interface IRegistry
{
    public void For<T>(Action<IListener<T>> configure);
}

public interface IListener<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>;
}