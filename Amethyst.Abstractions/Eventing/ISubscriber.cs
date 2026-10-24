namespace Amethyst.Abstractions.Eventing;

public interface ISubscriber
{
    public void Subscribe(IRegistry registry);
}