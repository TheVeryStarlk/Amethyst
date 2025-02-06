namespace Amethyst.Abstractions.Eventing;

public interface IRegistry
{
    public void For<T>(Action<IConsumer<T>> configure);
}