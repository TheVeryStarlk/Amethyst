namespace Amethyst.Eventing;

public sealed class Registry(Dictionary<Type, List<Delegate>> events)
{
    public void For<T>(Action<Consumer<T>> configure)
    {
        var consumer = new Consumer<T>(events);
        configure(consumer);
    }
}