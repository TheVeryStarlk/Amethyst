using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions;

public interface IClient
{
    public void Stop(Message message);
}