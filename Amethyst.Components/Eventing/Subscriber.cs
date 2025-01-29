namespace Amethyst.Components.Eventing;

// Perhaps consider renaming this to avoid confusion.
public abstract class Subscriber
{
    public abstract void Subscribe(IRegistry registry);
}

public interface ISubscriber<TSource>
{
    public void On<TEvent>(TaskDelegate<TSource, TEvent> callback) where TEvent : Event<TSource>;
}