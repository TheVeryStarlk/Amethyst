namespace Amethyst.Eventing;

public interface ISubscriber
{
    public void Subscribe(IRegistry registry);
}