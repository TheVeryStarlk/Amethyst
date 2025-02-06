using Amethyst.Abstractions.Eventing;

namespace Amethyst.Abstractions.Hosting;

public interface IAmethystBuilder
{
    public IAmethystBuilder AddSubscriber<T>() where T : class, ISubscriber;
}