using Amethyst.Components.Eventing;

namespace Amethyst.Components.Hosting;

public interface IAmethystBuilder
{
    public IAmethystBuilder AddSubscriber<T>() where T : class, ISubscriber;
}