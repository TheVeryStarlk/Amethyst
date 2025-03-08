using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorldManager WorldManager { get; }

    public void Stop();
}