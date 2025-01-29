namespace Amethyst.Hosting.Subscriber;

public interface ISubscriber
{
    public void Subscribe(IRegistry registry);
}

public interface ISubscriber<T>
{
    public void On<TEvent>(Action<T, TEvent> callback) where TEvent : Event<T>;
}