namespace Amethyst.Eventing;

public interface IRegistry
{
    public void For<T>(Action<IConsumer<T>> configure);
}

internal sealed class Registry(Dictionary<Type, List<Delegate>> events) : IRegistry
{
    public void For<T>(Action<IConsumer<T>> configure)
    {
        var consumer = new Consumer<T>(events);
        configure(consumer);
    }
}