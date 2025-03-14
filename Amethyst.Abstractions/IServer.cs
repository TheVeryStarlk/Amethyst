using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorldCollection Worlds { get; }

    public void Stop();
}