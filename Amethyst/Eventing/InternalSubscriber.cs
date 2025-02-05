using Amethyst.Components;
using Amethyst.Eventing.Sources.Servers;

namespace Amethyst.Eventing;

internal sealed class InternalSubscriber : ISubscriber
{
    public static void Register(Registry registry)
    {
        var subscriber = new InternalSubscriber();
        subscriber.Subscribe(registry);
    }

    public void Subscribe(IRegistry registry)
    {
        registry.For<Server>(consumer => consumer.On<StatusRequest>((_, request, _) =>
        {
            request.Status = Status.Create("A", 3, 1, 2, "a", "");
            return Task.CompletedTask;
        }));
    }
}