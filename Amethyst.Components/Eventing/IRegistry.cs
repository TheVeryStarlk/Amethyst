namespace Amethyst.Components.Eventing;

public interface IRegistry
{
    public void For<T>(Action<IConsumer<T>> configure);
}