using Amethyst.Components.Messages;

namespace Amethyst.Abstractions;

public interface IClient
{
    public void Stop(Message message);
}