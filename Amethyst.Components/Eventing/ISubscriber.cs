namespace Amethyst.Components.Eventing;

// Perhaps consider renaming this to avoid confusion.
public interface ISubscriber
{
    public void Subscribe(IRegistry registry);
}

public interface ISubscriber<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>;
}