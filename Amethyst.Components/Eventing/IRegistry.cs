namespace Amethyst.Components.Eventing;

public interface IRegistry
{
    public void For<T>(Action<ISubscriber<T>> configure);
}