using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorldService WorldManager { get; }

    public void Stop();
}